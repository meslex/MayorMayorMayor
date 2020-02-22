using System;
using UnityEngine;
using System.Collections.Generic;

public class Target
{
    private GameObject gameObj;
    private Transform transform;
    private string name;
    private Health hp;


    public string Name { get { return name; } }
    public Transform Transform { get { return transform; } }
    public GameObject Obj { get { return gameObj; } }
    public Health HP { get { return hp; } }


    public Target(GameObject obj)
    {
        this.gameObj = obj;
        this.transform = obj.transform;
        this.name = obj.name;
        hp = obj.GetComponent<Health>();
    }

    public override bool Equals(object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Target t = (Target)obj;
            return (gameObj == t.gameObj);
        }
    }
}