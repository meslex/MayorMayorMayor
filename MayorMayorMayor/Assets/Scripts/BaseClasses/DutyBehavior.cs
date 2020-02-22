using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DutyBehavior : MonoBehaviour
{
    public GameObject cigarette;
    public GameObject smoke;
    public float turnSmoothing;

    private Controller controller;

    public void Awake()
    {
        controller = GetComponent<Controller>();   
    }

    public void Start()
    {
        if (cigarette != null)
        {
            cigarette.SetActive(false);
            smoke.SetActive(false);
        }

    }

    private void OnTriggerStay(Collider col)
    {
        if(col.tag == "DutyPoint")
        {
            if (cigarette != null)
            {
                cigarette.SetActive(true);
                smoke.SetActive(true);
                controller.CanApproachTheTarget = false;
                //controller.PermissionToKillOnSight = true;//give permission to kill;
            }
            if(controller.State != AgentState.Attack)
                transform.rotation = Quaternion.Lerp(transform.rotation, col.transform.rotation, Time.deltaTime * turnSmoothing);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "DutyPoint")
        {
            if (cigarette != null)
            {
                cigarette.SetActive(false);
                smoke.SetActive(false);
            }
        }
    }


}
