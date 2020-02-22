using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NavigationMovement : MonoBehaviour
{
    public delegate void ReachedAction();
    public static event ReachedAction OnReached;

    public Vector3[] waypoints;
    public LayerMask waypointsLayer;
    public LayerMask cameraBounds;
    public float turnSmoothing = 0.5f;
    public float switchDistance;
    public float arrivalDistance;
    public float defaultSpeed;
    public bool cycle = true;

    protected Vector3 point;
    protected NavMeshAgent agent;
    protected Health hp;
    protected Controller controller;
    protected Animator animator;
    protected Rigidbody rb;


    protected bool moving;
    protected int currentWaypoint;
    protected bool reached;
    protected float speed;
    protected Vector3 destinationTarget;
    protected float currentSwitchDistance;
    protected AgentState currentMovingState;

    protected readonly int hashSpeedPara = Animator.StringToHash("speed");

    void Update()
    {
        animator.SetFloat(hashSpeedPara, speed);
        if(!reached)
            transform.rotation = Quaternion.LookRotation(agent.velocity, Vector3.up);

        //prevents object from flying/falling through ground
        //if (!hp.IsDead)
            //transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        if (controller.State == AgentState.Reached)
            rb.angularVelocity = Vector3.zero;
    }

    private void Awake()
    {
        controller = GetComponent<Controller>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        hp = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        currentSwitchDistance = switchDistance;
        currentWaypoint = 0;
        reached = false;

        agent.speed = defaultSpeed;
        agent.autoBraking = false;
        agent.stoppingDistance = 0f;

    }

    //nav mesh
    public void WaypointMoving()
    {
        if (controller.State == AgentState.Reached)
            return;

        if (controller.State == AgentState.Attack)
        {
            ActivateWaypointMovement(true);

        }
        float temp = Vector3.Distance(waypoints[currentWaypoint], transform.position);

        if (temp <= currentSwitchDistance)
        {
            if (waypoints.Length <= 0)
                ReachedDestination();
            else
                GotoNextPoint();
        }

        speed = defaultSpeed;
        animator.SetFloat(hashSpeedPara, speed);
    }

    private void FindPointerToClosestWaypoint()
    {
        float dist = (waypoints[0] - transform.position).sqrMagnitude;
        float temp = 0;
        for (int i = 1; i < waypoints.Length; ++i)
        {
            temp = (waypoints[i] - transform.position).sqrMagnitude;
            currentWaypoint = i;
            if (temp < dist)
            {
                dist = temp;
            }
            else
            {
                break;
            }
        }
    }

    private void ActivateWaypointMovement(bool setPreviousState = false)
    {
        if (waypoints.Length > 1)
            FindPointerToClosestWaypoint();
        else
            currentWaypoint = 0;
        agent.SetDestination(waypoints[currentWaypoint]);
        agent.isStopped = false;
        if (setPreviousState)
            controller.State = currentMovingState;
        else
            currentMovingState = controller.State;


    }

    public void SetPath(Vector3[] path, bool cycle = false, AgentState state = AgentState.Patrolling)
    {
        reached = false;
        waypoints = path;
        this.cycle = cycle;
        currentSwitchDistance = switchDistance;
        ActivateWaypointMovement();
    }

    public void SetPath(Vector3 target, AgentState state = AgentState.Patrolling)
    {
        reached = false;
        waypoints = new Vector3[] { target };
        this.cycle = false;
        currentSwitchDistance = arrivalDistance;
        ActivateWaypointMovement();
    }

    private void GotoNextPoint()
    {
        if (waypoints.Length == 0)
            return;

        if (cycle)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
        else
        {
            if (currentWaypoint >= waypoints.Length - 1)
            {
                ReachedDestination();
                return;
            }


            currentWaypoint++;
        }

        agent.SetDestination(waypoints[currentWaypoint]);
    }

    protected virtual void ReachedDestination(Target target = null)
    {
        reached = true;
        controller.State = AgentState.Reached;
        speed = 0f;
        moving = false;
        agent.isStopped = true;
        animator.SetFloat(hashSpeedPara, speed);
        rb.angularVelocity = Vector3.zero;
    }

    public void SetAgentSpeed(bool defaultSpeed, float speed = 0)
    {
        if (!defaultSpeed)
            agent.speed = speed;
        else
            agent.speed = this.defaultSpeed;
    }

    public void TurnOffAgent()
    {
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        reached = true;
    }

    public virtual void MoveToTarget(Target target, float arrivalRadius = 0)
    {
        reached = false;
        agent.SetDestination(target.Transform.position);
        agent.isStopped = false;
    }

}

