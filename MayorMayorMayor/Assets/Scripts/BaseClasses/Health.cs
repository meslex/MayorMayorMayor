using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private float currentHealth;
    private bool dead;


    protected Animator animator;
    //protected Controller controller;
    protected Attack attack;
    protected List<Health> attackersMelee = new List<Health>();// i want to keep track of how many enemies can simultaneously attack target 
    protected List<Health> attackersRange = new List<Health>();

    //public GameObject bloodStain;
    public float initialHealth;
    public UnitType Type;
    public int numberOfPossibleMeleeAttackers;
    public int numberOfPossibleRangeAttackers;
    public float timeBeforeBodyRemoval;
    public bool changeTargetWhenAttacked;
    public float CurrentHeath { get { return currentHealth; } }

    public Slider slider;
    public Image fillImage;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;

    public bool IsDead { get { return dead; } }

    private void Awake()
    {
        //controller = GetComponent<Controller>();
        animator = GetComponent<Animator>();
        attack = GetComponent<Attack>();

    }

    protected virtual void Start()
    {
        dead = false;

        currentHealth = initialHealth;
        if (numberOfPossibleMeleeAttackers == 0)
            numberOfPossibleMeleeAttackers++;

        slider.maxValue = initialHealth;
        slider.value = initialHealth;

        SetHealthUI();
    }

    private void SetHealthUI()
    {
        slider.value = currentHealth;

        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, currentHealth / initialHealth);
    }

    public void RemoveAttacker(List<Health> ls)
    {
        for (int i = 0; i < ls.Count; ++i)
        {
            if (ls[i] != null)
            {
                if (ls[i].IsDead)
                {
                    ls.RemoveAt(i);
                    --i;
                }
            }
            else
            {
                ls.RemoveAt(i);
                --i;
            }
        }
    }

    public virtual void ReceiveDamage(float damage, GameObject attacker = null)
    {
        if (!dead)
        {
            currentHealth -= damage;

            SetHealthUI();
            //Debug.Log($"Damage received current health {currentHealth}");

            if (currentHealth <= 0f)
                Die();
        }
    }

    protected virtual void Die()
    {

        dead = true;

        animator.SetBool("dead", true);
        animator.Play("Death");
        GetComponent<Rigidbody>().isKinematic = true;
        StartCoroutine(RemoveBody());
    }

    IEnumerator RemoveBody()
    {
        yield return new WaitForSeconds(timeBeforeBodyRemoval);


        GetComponent<NavMeshAgent>().enabled = false;
        while (transform.position.y > -8)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, -10, transform.position.z), 0.5f * Time.deltaTime);

            yield return null;
        }

        Destroy(gameObject);
    }

    public Target PermissionToAttackMelee(GameObject obj)
    {
        if (!dead)
            return new Target(gameObject);
        else
            return null;
    }

    public Target PermissionToAttackRange(GameObject obj)
    {
        if (!dead)
            return new Target(gameObject);
        else
            return null;
    }

}