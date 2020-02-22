using System;
using UnityEngine;
using System.Collections;

public class SoldierHealth : Health
{
    public MeshRenderer renderer;
    protected GameObject attacker;
    protected FriendlyUnitController controller;
    public Material deadMaterial;
    protected Material currentMat;
    protected UnitsController unitsController;
    //public Material hurtMaterial;

    protected override void Start()
    {
        unitsController = GameObject.Find("GameController").GetComponent<UnitsController>();
        controller = GetComponent<FriendlyUnitController>();
        base.Start();
    }



    public override void ReceiveDamage(float damage, GameObject attacker = null)
    {
        Debug.Log("damage received!");
        currentMat = renderer.material;
        renderer.material = deadMaterial;
        StartCoroutine(RemoveHurtColor());

        if (unitsController != null && changeTargetWhenAttacked)
        {
            //if(this.attacker != attack || this.attacker == null)
            //unitsController.UnitAttacked(controller, new Target(attacker));
        }


        base.ReceiveDamage(damage, attacker);
    }

    protected override void Die()
    {
        Debug.Log("Soldier is dead;");
        renderer.material = deadMaterial;
        base.Die();
    }

    IEnumerator RemoveHurtColor()
    {
        yield return new WaitForSeconds(0.2f);
        if (!IsDead)
            renderer.material = currentMat;
    }
}
