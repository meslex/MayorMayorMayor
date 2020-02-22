using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Attack // animation synchronization
{
    public override void ComeAndKill()
    {

        PursueEnemy();

        controller.State = AgentState.Attack;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.Transform.position - transform.position), Time.deltaTime * 5f);
        float angle = Vector3.Angle(transform.forward, target.Transform.position - transform.position);
        if (angle > 25f)
            return;

        float dist = Vector3.SqrMagnitude(transform.position - target.Transform.position);

        if (dist < attackRadius * attackRadius)
        {

            if (Time.time > nextAttack)
            {
                animator.SetBool(hashAttackPara, true);
                nextAttack = Time.time + attackingSpeed;
                StartCoroutine(AttackDelay());

            }
        }
        else if (dist > removeTargetDist)
        {
            RemoveTarget();
        }

    }



    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(attackingSpeed);
        if (target == null || target.HP.IsDead)
        {
            target = null;
            animator.SetBool(hashAttackPara, false);
        }
        else
        {
            target.HP.ReceiveDamage(damage, gameObject);
        }


    }

    public override void PursueEnemy()
    {
        if (movement != null && target != null)
        {
            if ((transform.position - target.Transform.position).sqrMagnitude > attackRadius * attackRadius)
                movement.MoveToTarget(target);
            else
            {
                movement.TurnOffAgent();
            }

        }
    }
}

