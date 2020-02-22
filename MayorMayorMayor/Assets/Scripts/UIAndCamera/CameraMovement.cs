using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float speed;
    public float percent;
    public bool movingCameraWithMouse;
    public ChoosingCircle circle;

    private Vector3 m_MoveVelocity;
    private Rigidbody rb;
    private float xMargin;
    private float yMargin;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //Debug.Log("Screen Height : " + Screen.height);
        //Debug.Log("Screen Width : " + Screen.width);
        xMargin = Screen.width * percent / 100;
        yMargin = Screen.height * percent / 100;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(circle != null && !circle.CurrentlyInSlowMo)
        {
            MoveCamera();
        }

    }

    void Update()
    {
        if (circle != null && circle.CurrentlyInSlowMo)
        {
            MoveCamera();
        }

    }

    private void MoveCamera()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (movingCameraWithMouse)
        {
            //Debug.Log($"Mouse pos: { Input.mousePosition.ToString()}");
            if (Input.mousePosition.x < xMargin)
                horizontal = -1;

            if (Input.mousePosition.x > Screen.width - xMargin)
                horizontal = 1;

            if (Input.mousePosition.y < yMargin)
                vertical = -1;

            if (Input.mousePosition.y > Screen.height - yMargin)
                vertical = 1;
        }

        Vector3 deltaX = new Vector3(horizontal, 0, 0) * Time.deltaTime * speed;
        Vector3 deltaZ = new Vector3(0, 0, vertical) * Time.deltaTime * speed;


        transform.position = transform.position + deltaX;
        transform.position = transform.position + deltaZ;

        rb.velocity = Vector3.zero;
    }
}
