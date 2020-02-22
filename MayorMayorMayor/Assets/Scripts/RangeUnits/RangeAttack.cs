using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttack : Attack
{
    public ParticleSystem muzzleFlash;
    public GameObject fleshHitEffect;
    public ParticleSystem cartridgeEjection;
    public Animator rifleAnimation;


    public override void ComeAndKill(bool canApproachTarget)
    {
        if (canApproachTarget)
            PursueEnemy();

        controller.State = AgentState.Attack;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.Transform.position - transform.position), Time.deltaTime * 5f);
        float angle = Vector3.Angle(transform.forward, target.Transform.position - transform.position);
        if (angle > 25f)
            return;

        float dist = Vector3.Distance(transform.position, target.Transform.position);
        if (dist < attackRadius)
        {
            //attack
            if (Time.time > nextAttack && permissionToKill)
            {
                nextAttack = Time.time + attackingSpeed;
                if (audioSource != null)
                    audioSource.PlayOneShot(audioSource.clip);
                rifleAnimation.SetTrigger("shoot");
                muzzleFlash.Play();
                cartridgeEjection.Play();
                StartCoroutine(StopFiring());
                target.HP.ReceiveDamage(damage, gameObject);
            }
            //Debug.DrawLine(transform.position, target.Transform.position, Color.black);
        }
        else if (dist > removeTargetDist)
        {
            RemoveTarget();
        }

        if (target == null || target.HP.IsDead)
        {
            RemoveTarget();
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

    private IEnumerator StopFiring()
    {
        yield return new WaitForSeconds(1f);
        muzzleFlash.Stop();
        cartridgeEjection.Stop();

    }

    public override void RemoveTarget()
    {
        muzzleFlash.Stop();
        cartridgeEjection.Stop();
        audioSource.Stop();

        base.RemoveTarget();
    }



}