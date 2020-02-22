using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMovement : NavigationMovement
{
    protected float margin = 0.5f;

    public override void MoveToTarget(Target target, float arrivalRadius = 0)// this shit is bad, like really bad
    {
        controller.State = AgentState.Pursuit;
        float dist = Vector3.Distance(target.Transform.position, transform.position);

        //Debug.Log("Distance to target: " + Mathf.Round(dist));

        if (dist >= arrivalRadius - margin)//here we use navmeshTo move to target
        {

            speed = defaultSpeed;
            agent.isStopped = false;
            Vector3 temp = target.Transform.position;
            Debug.DrawLine(transform.position, temp, Color.red);
            agent.SetDestination(temp);
            agent.speed = speed;
            moving = true;
            //agent.stoppingDistance = arrivalRadius;
            Quaternion targetRotation = Quaternion.LookRotation(agent.desiredVelocity);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSmoothing * Time.deltaTime);;
        }
        else if (dist < arrivalRadius)//stop moving and turn to target
        {
            agent.isStopped = true;
            speed = 0;
            moving = false;
            //transform.position += currentVelocity.normalized * defaultSpeed * Time.deltaTime;
            //transform.rotation =  Quaternion.LookRotation(target.Transform.position - transform.position);
        }

    }

}
