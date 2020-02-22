using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnController : MonoBehaviour
{
    public PauseController pause;

    public Text oilText;
    public float initialOil;
    public float spawnSpeed;

    public float[] price;
    public GameObject[] units;
    public Transform[] spawnPoints;

    public Transform[] attackPath;
    public Transform[] patrolPath;
    public Transform assemblyPoint;

    protected float currentOil;
    protected float nextSpawn;
    protected GameObject unit;

    protected Vector3[] attackPathVec;
    protected Vector3[] patrolPathVec;
    protected Vector3 assemblyPointVec;


    public float Oil { get { return currentOil; } }

    private void Awake()
    {
        pause = GetComponent<PauseController>();
    }

    private void Start()
    {
        ConvertTransformToVector3();
        currentOil = initialOil;
    }

    private void Update()
    {
        if (pause.IsPaused)
            return;

        oilText.text = "Oil: " + currentOil;

        if (Input.GetButtonDown("One") && Time.time > nextSpawn)
        {
            AttemptSpawn(0);
        }

        if (Input.GetButtonDown("Two") && Time.time > nextSpawn)
        {
            //AttemptSpawn(1);
        }
    }

    protected virtual void ConvertTransformToVector3()
    {
        attackPathVec = new Vector3[attackPath.Length];
        patrolPathVec = new Vector3[patrolPath.Length];

        for (int i = 0; i < attackPath.Length; ++i)
        {
            attackPathVec[i] = attackPath[i].position;
        }

        for (int i = 0; i < patrolPath.Length; ++i)
        {
            patrolPathVec[i] = patrolPath[i].position;
        }

        assemblyPointVec = assemblyPoint.position;
    }

    protected void AttemptSpawn(int unitNumber)
    {
        if (currentOil >= price[unitNumber])
        {
            nextSpawn = Time.time + spawnSpeed;
            currentOil -= price[unitNumber];
            Spawn(units[unitNumber], spawnPoints[unitNumber]);
        }
    }

    protected virtual void Spawn(GameObject obj, Transform pos)
    {
        unit = Instantiate(obj, pos.position, pos.rotation);
        unit.GetComponent<FriendlyUnitController>().SetInfo(attackPathVec,  patrolPathVec,  assemblyPointVec);
    }

    public void AddOil(float amount)
    {
        currentOil += amount;
        //Debug.Log(amount + "oil was added; Current amount: " + currentOil);
    }

}


