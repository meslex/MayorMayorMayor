using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FriendlyUnitController : Controller
{
    public float retreatSpeed = 8;
    //protected new NavigationMovement movement;

    [HideInInspector]
    public Vector3[] attackPath;
    [HideInInspector]
    public Vector3[] patrolPath;
    [HideInInspector]
    public Vector3 duty;
    [HideInInspector]
    public Vector3 assemblyPoint;

    public Image highlightImage;
    public Color highlightColor = Color.cyan;
    public Color selectedColor = Color.green;
    public Color normalColor = Color.clear;

    private bool permissionToRemoveRetret;

    protected virtual void Behavior()
    {
        if (!health.IsDead)
        {
            if (retreat)
            {
                IsClear();
            }
            if (PermissionToKillOnSight)
            {
                attack.LookForTargets();

                if (attack.Target == null)
                {
                    movement.WaypointMoving();
                }
                else
                {
                    if (!attack.Target.HP.IsDead)
                        attack.ComeAndKill(CanApproachTheTarget);
                    else
                        attack.RemoveTarget();
                }
            }
            else
            {
                movement.WaypointMoving();
            }
        }

    }

    protected override void Awake()
    {
        unitsController = GameObject.Find("GameController").GetComponent<UnitsController>();
        base.Awake();
    }

    protected override void Start()
    {
        State = AgentState.JustBorn;

        unitsController.IncreaseNumberOfSoldiers(this);

        ReportToBase();

        retreat = false;
        permissionToRemoveRetret = false;
    }

    protected override void Update()
    {
        Behavior();
    }



    public void SetInfo(Vector3[] attackPath, Vector3[] patrol, Vector3 assembly)
    {
        this.attackPath = attackPath;
        this.patrolPath = patrol;
        this.assemblyPoint = assembly;
    }

    public void ChangeMaterial(MaterialState state)
    {
        if (health.IsDead)
            return;

        if (state == MaterialState.NormalMaterial)
        {
            highlightImage.color = normalColor;
        }
        else if (state == MaterialState.HighlightMaterial)
        {
            highlightImage.color = highlightColor;
        }
        else if (state == MaterialState.SelectedMaterial)
        {
            highlightImage.color = selectedColor;
        }

    }

    private void OnDisable()
    {
        unitsController.DecreaseNumberOfSoldiers(this);
    }

    public bool IsClear()
    {
        if (!attack.FindTarget())
        {
            RemoveRetreat();
            return true;
        }

        else
        {
            attack.RemoveTarget();
            return false;
        }

    }

    private void RemoveRetreat()
    {
        movement.SetAgentSpeed(true);
        retreat = false;
        ReportToBase();
    }

    public void Attack()
    {
        Debug.Log("Attack signal was received!");

        if (permissionToRemoveRetret)
            RemoveRetreat();

        if (!health.IsDead && !retreat)
        {
            State = AgentState.OnAttackPath;
            movement.SetPath(attackPath, false);
            PermissionToKillOnSight = true;
            CanApproachTheTarget = true;
            attack.RemoveTarget();
        }

    }

    public void Retreat()
    {
        Debug.Log("Retreat signal was received!");
        if (!health.IsDead)
        {
            State = AgentState.Retreat;
            retreat = true;
            PermissionToKillOnSight = false;
            CanApproachTheTarget = false;

            movement.SetAgentSpeed(false, retreatSpeed);
            StartCoroutine(GivePermissionToRemoveRetreat());
            movement.SetPath(assemblyPoint);
            attack.RemoveTarget();
        }
    }

    IEnumerator GivePermissionToRemoveRetreat()
    {
        yield return new WaitForSeconds(3f);
        permissionToRemoveRetret = true;
    }

    public void Patrol()
    {
        Debug.Log("Patrol signal was received!");

        if (!health.IsDead && !retreat)
        {
            State = AgentState.OnPatrolPath;
            PermissionToKillOnSight = false;
            CanApproachTheTarget = false;
            movement.SetPath(patrolPath, true);
            attack.RemoveTarget();
        }
    }

    public void Duty(Transform point)
    {
        Debug.Log("Duty signal was received!");

        if (!health.IsDead && !retreat)
        {
            State = AgentState.OnPatrolPath;
            PermissionToKillOnSight = true;
            CanApproachTheTarget = true;
            movement.SetPath(point.position);
            attack.RemoveTarget();
        }
    }

    public void ChangeTarget(Target target)
    {
        if (!health.IsDead)
        {
            attack.ChangeTarget(target);
        }
    }

    public void ReportToBase()
    {
        if (!health.IsDead)
        {
            State = AgentState.Patrolling;
            PermissionToKillOnSight = true;
            CanApproachTheTarget = true;
            movement.SetPath(assemblyPoint);
            attack.RemoveTarget();
        }
    }

    public void MoveTo(Transform point, AgentState state = AgentState.Patrolling)
    {
        Debug.Log("MoveTo signal was received!");
        State = state;
        if (!health.IsDead && !retreat)
            movement.SetPath(point.position);
    }

    public void MoveTo(Vector3 point, AgentState state = AgentState.Patrolling)
    {
        Debug.Log("MoveTo signal was received!");
        State = state;
        if (!health.IsDead && !retreat)
            movement.SetPath(point);
    }
}