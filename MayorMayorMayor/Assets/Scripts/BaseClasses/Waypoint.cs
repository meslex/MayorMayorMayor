using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public bool hide;

    public Waypoint leftNeighbour;
    public Waypoint rightNeighbour;


    protected MeshRenderer mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();

        if (hide)
        {
            HideMesh();
        }
        

    }

    public void HideMesh()
    {
        mesh.enabled = false; 
    }


}
