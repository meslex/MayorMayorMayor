using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilPump : MonoBehaviour
{
    public SpawnController controller;
    public float pumpingSpeed;
    public float amount;

    private float nextPortion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > nextPortion)
        {
            nextPortion = Time.time + pumpingSpeed;
            controller.AddOil(amount);
        }
    }
}
