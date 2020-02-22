using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{

    public float attackRadius;
    public float removeTargetDist;
    public float sightDistance;
    public LayerMask enemyLayer;
    public float damage;
    public float attackingSpeed;
    public bool permissionToKill;
    public AttackType type;

    protected Target target;
    protected NavigationMovement movement;
    protected Animator animator;
    protected Controller controller;
    protected AudioSource audioSource;
    protected float nextAttack;

    protected readonly int hashAttackPara = Animator.StringToHash("attack");

    public Target Target { get { return target; } }

    private void Awake()
    {
        movement = GetComponent<NavigationMovement>();
        animator = GetComponent<Animator>();
        controller = GetComponent<Controller>();
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void RemoveTarget()
    {
        target = null;
    }


    public virtual bool FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sightDistance, enemyLayer);

        for (int i = 0; i < hitColliders.Length; ++i)
        {
            if (type == AttackType.Melee)
            {
                target = hitColliders[i].GetComponent<Health>().PermissionToAttackMelee(gameObject);
            }
            else
            {
                target = hitColliders[i].GetComponent<Health>().PermissionToAttackRange(gameObject);
            }

            if (target != null)
                return true;
        }

        return false;
    }

    public virtual bool LookForTargets()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sightDistance, enemyLayer);

        for (int i = 0; i < hitColliders.Length; ++i)
        {
            Health newTarget = hitColliders[i].GetComponent<Health>();
            if (target == null)
            {
                if (type == AttackType.Melee)
                {
                    target = newTarget.PermissionToAttackMelee(gameObject);
                }
                else
                {
                    target = newTarget.PermissionToAttackRange(gameObject);
                }
                return true;
            }
            else
            {
                if(target.HP.Type == UnitType.Boss && newTarget.Type != UnitType.Boss)
                {
                    if (type == AttackType.Melee)
                    {
                        target = newTarget.PermissionToAttackMelee(gameObject);
                    }
                    else
                    {
                        target = newTarget.PermissionToAttackRange(gameObject);
                    }
                    return true;
                }
            }
            
        }

        return false;
    }

    protected bool IsTargetInLineOfSight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, enemyLayer))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.black);
            Debug.Log("Target is in direct line of sight");
            return true;
        }


        return false;
    }

    public void ChangeTarget(Target newTarget)
    {
        Debug.Log("Responds to ChangeTarget!");

        if (target == null)
        {
            if (target.HP.CurrentHeath / target.HP.initialHealth > 0.25f)
                target = newTarget;

        }
        else if(newTarget != target)
        {
            target = newTarget;
        }
    }

    public virtual void PursueEnemy() { }

    public virtual void ComeAndKill() { }

    public virtual void ComeAndKill(bool canApproachTheTarget)
    {
        ComeAndKill();
    }

}

public enum AttackType
{
    Range,
    Melee
}
