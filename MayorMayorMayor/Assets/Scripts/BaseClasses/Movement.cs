using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public Transform[] waypoints;
    public LayerMask waypointsLayer;
    public LayerMask cameraBounds;
    public float switchDistance;
    public float defaultSpeed;
    public bool cycle = true;

 
    protected float steeringForce = 0.7f;
    protected float avoid_force = 0.9f;
    protected float turnSmoothing = 0.5f;
    protected float slowingSpeed = 0.175f;
    protected float deltaAngelThreshold = 45f;
    protected float sightDistance = 10f;
    protected float separationWeight = 0.3f;

    protected NavMeshAgent agent;
    protected Health hp;
    protected Controller controller;
    protected Animator animator;
    protected Rigidbody rb;

    protected LayerMask layerMask;
    protected int currentWaypoint;
    protected bool reached;
    protected Vector3 currentVelocity;
    protected bool moving;
    protected Vector3 steering;
    protected float speed;
    protected bool stuck;

    protected readonly int hashSpeedPara = Animator.StringToHash("speed");

    public bool IsMoving { get { return moving; } }

    private void Awake()
    {
        controller = GetComponent<Controller>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        hp = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {

        currentWaypoint = 0;
        reached = false;
        currentVelocity = transform.forward;

        agent.speed = defaultSpeed;
        agent.autoBraking = false;
        agent.stoppingDistance = 0f;

        layerMask = 1 << gameObject.layer;

        //sqrSwitchDistance = switchDistance * switchDistance;
    }

    void Update()
    {
        animator.SetFloat(hashSpeedPara, speed);

        //prevents object from flying/falling through ground
        if (!hp.IsDead)
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        if (controller.State == AgentState.Reached)
            rb.angularVelocity = Vector3.zero;
    }

    //Through vectors from point to point obstacle avoidance is low
    protected void Move(Transform target)
    {
        speed = defaultSpeed;
        moving = true;

        // currentVelocity = desiredVelocity !!!!?
        Vector3 desiredVelocity = (target.position - transform.position).normalized;
        Vector3 steering = (desiredVelocity - currentVelocity) * steeringForce * Time.deltaTime;

        currentVelocity += steering + ObstacleAvoidance() + MaintainSeparation();

        Debug.DrawRay(transform.position, transform.position + currentVelocity.normalized * speed * Time.deltaTime, Color.magenta);

        transform.position += currentVelocity.normalized * speed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);

        float deltaAngle = Quaternion.Angle(targetRotation, transform.rotation);

        if (deltaAngle >= deltaAngelThreshold)
        {
            transform.rotation = Quaternion.Slerp(targetRotation, transform.rotation, turnSmoothing);
        }
        else
        {
            transform.rotation = targetRotation;
        }

    }

    //wierd jump effects and transition from this to waypoint movements
    /*public void FindNearestWaypoint()
    {
        if(controller.State == AgentState.LookingForWaypoint)
        {
            //Calculate distance to waypoint change state
            if (agent.pathPending)
                return;

            if(Vector3.Distance(transform.position, desiredWaypoint.position) < 0.5f)
            {
                agent.isStopped = true;
                controller.State = AgentState.Reached;
            }
        }
        else
        {
            float dist = 0;
            int pt = 0;
            Collider[] hitbuffer = Physics.OverlapSphere(transform.position, 150f, waypointsLayer);
            if(hitbuffer.Length > 0)
            {
                dist = Vector3.Distance(transform.position, hitbuffer[0].transform.position);
                for (int i = 1; i < hitbuffer.Length; ++i)
                {
                    if (Vector3.Distance(transform.position, hitbuffer[i].transform.position) < dist)
                    {
                        dist = Vector3.Distance(transform.position, hitbuffer[i].transform.position);
                        pt = i;
                    }
                }
                desiredWaypoint = hitbuffer[pt].transform;
                agent.SetDestination(desiredWaypoint.position);
                agent.speed = defaultSpeed;
                agent.stoppingDistance = 0;
                //agent.autoBraking = false;
                agent.isStopped = false;
                controller.State = AgentState.LookingForWaypoint;
            }
            
        }
    }*/

    //needs work
    protected Vector3 ObstacleAvoidance()
    {
        RaycastHit hit;
        Vector3 avoidance = Vector3.zero;
        LayerMask mask = ~(layerMask | waypointsLayer | cameraBounds);
        if (Physics.SphereCast(transform.position, 2f, transform.forward, out hit, sightDistance, mask))
        {
            if(hit.distance < 0.2f)
            {
                //FindNearestWaypoint();
                Debug.Log("Enable WaypointFind Function, Obstacle: " + hit.transform.gameObject.name);
            }
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            //Debug.Log("Did Hit");
            avoidance = transform.forward - hit.transform.position;
            avoidance = avoidance.normalized * avoid_force * Time.deltaTime;

        }

        return avoidance;
    }

    //for vector movement
    public virtual void WaypointMoving()
    {

        agent.isStopped = true;
        controller.State = AgentState.Patrolling;

        if (waypoints.Length == 0 || reached)
            return;

        if (cycle)
        {
            if ((waypoints[currentWaypoint].position - transform.position).magnitude <= switchDistance)
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
        else
        {
            if ((waypoints[currentWaypoint].position - transform.position).magnitude <= switchDistance)
                currentWaypoint++;
        }

        if (currentWaypoint >= (waypoints.Length))
        {
            ReachedDestination();
            return;
        }


        Move(waypoints[currentWaypoint]);

    }

    protected Vector3 MaintainSeparation()
    {
        Vector3 separation = Vector3.zero;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5, layerMask);

        if (hitColliders.Length <= 1)
            return separation;

        for (int i = 0; i < hitColliders.Length; ++i)
        {

            separation += transform.position - hitColliders[i].transform.position;

        }


        separation /= hitColliders.Length - 1;
        Debug.DrawRay(transform.position, separation, Color.blue);
        separation = separation.normalized;

        separation *= separationWeight * Time.deltaTime;

        return separation;
    }

    protected Vector3 MaintainSeparation(float weight)
    {
        Vector3 separation = Vector3.zero;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5,  layerMask);

        if (hitColliders.Length <= 1)
            return separation;

        for (int i = 0; i < hitColliders.Length; ++i)
        {

            separation += transform.position - hitColliders[i].transform.position;

        }


        separation /= hitColliders.Length - 1;
        Debug.DrawRay(transform.position, separation, Color.blue);
        separation = separation.normalized;

        separation *= weight * Time.deltaTime;

        return separation;
    }

    protected virtual void ReachedDestination(Target target = null)
    {
        reached = true;
        controller.State = AgentState.Reached;
        speed = 0f;
        moving = false;
        agent.isStopped = true;
        animator.SetFloat(hashSpeedPara, speed);
        currentVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (target != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.Transform.position - transform.position);

            transform.rotation = Quaternion.Slerp(targetRotation, transform.rotation, turnSmoothing);

        }

    }

    public void TurnOffAgent()
    {
        if (agent.isOnNavMesh)
            agent.isStopped = true;
    }

    //this is for approaching the Target
    public virtual void MoveToTarget(Target target, float arrivalRadius = 0) { }
    public virtual void SetPath(Transform[] path, bool cycle = false) { }
    public virtual void SetPath(Vector3 point, bool cycle = false) { }
    public virtual void SetPath(Transform point, bool cycle = false) { }
}
