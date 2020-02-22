using System;
using UnityEngine;
using System.Collections.Generic;

public class Controller : MonoBehaviour
{

    [HideInInspector]
    public UnitsController unitsController { get; set; }
    [HideInInspector]
    public bool PermissionToKillOnSight { get; set; }
    [HideInInspector]
    public bool CanApproachTheTarget { get; set; }

    public bool computer;
    public MeshRenderer meshRenderer;

    public AgentState State { get; set; }

    protected NavigationMovement movement;
    protected Attack attack;
    protected Health health;

    protected bool retreat;



    public bool IsDead { get { return health.IsDead; } }

    protected virtual void Awake()
    {
        movement = GetComponent<NavigationMovement>();
        attack = GetComponent<Attack>();
        health = GetComponent<Health>();

    }

    protected virtual void Start()
    {
        State = AgentState.JustBorn;

    }

    protected virtual void Update()
    {
        RegularBehavior();
    }


    public virtual void RegularBehavior()
    {
        if (!health.IsDead)
        {
            if (attack.Target == null)
            {
                if (!attack.FindTarget())//Target was not found
                {
                    movement.WaypointMoving();
                }
            }
            else
            {
                Debug.Log(gameObject.name + " is locked on target: " + attack.Target.Name);
                if (!attack.Target.HP.IsDead)
                    attack.ComeAndKill();
                else
                    attack.RemoveTarget();
            }
        }
        else
        {
            movement.TurnOffAgent();
        }

    }
}