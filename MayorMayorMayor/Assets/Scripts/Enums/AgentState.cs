using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum AgentState
{
    MoveToPoint,
    Patrolling,
    Pursuit,
    Attack,
    LookingForWaypoint,
    Reached,
    JustBorn,
    OnDuty,
    OnAttackPath,
    OnPatrolPath,
    Retreat,

}