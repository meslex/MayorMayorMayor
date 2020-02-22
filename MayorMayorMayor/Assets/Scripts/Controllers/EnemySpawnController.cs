using System;
using System.Collections;
using UnityEngine;


public class EnemySpawnController : SpawnController
{
    private void Update()
    {
        if(Time.time > nextSpawn)
            AttemptSpawn(0);

    }

    protected override void ConvertTransformToVector3()
    {
        attackPathVec = new Vector3[attackPath.Length];


        for (int i = 0; i < attackPath.Length; ++i)
        {
            attackPathVec[i] = attackPath[i].position;
        }

    }

    protected override void Spawn(GameObject obj, Transform pos)
    {
        unit = Instantiate(obj, pos.position, pos.rotation);
        unit.GetComponent<NavigationMovement>().SetPath(attackPathVec, false, AgentState.OnAttackPath);
    }
}
