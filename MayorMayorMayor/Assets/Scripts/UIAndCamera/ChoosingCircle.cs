using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoosingCircle : MonoBehaviour
{
    public Mode mode;
    public UnitsController controller;
    public PauseController pauseController;
    public GameObject GreyPart;
    public GameObject circle;
    public Commands[] commands;

    public Sprite blue;
    public Sprite grey;
    public float sensitivity = 1.5f;
    public float newScale;
    public float smoothing;
    public float dist;//to exclude small mouse jiggling when working on both mode and using scrollWheel 
    public bool slowMo;
    public bool CurrentlyInSlowMo { get { return currentlyInSLowMo; } }
    public float slowMoSpeed;

    public const int NUMBER_OF_PIECES = 8;


    private GameObject[] segments = new GameObject[NUMBER_OF_PIECES];
    private GameObject[] segmentTitles;
    private Array enumData = Enum.GetValues(BasicCommandList.None.GetType());
    private Vector3 mousePos;
    private float currentScrollWheelPt;
    private int currentCirclePt;
    private int currentRealPt;
    private AudioSource audioSource;
    private float currentTimeScale;
    private bool currentlyInSLowMo;
    private int scalingDownPt;
    private int scalingUpPt;
    private Vector3 upscale;
    private bool change;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        circle.GetComponent<Image>().enabled = false;
        CreateCircle();
        upscale = new Vector3(newScale, newScale, newScale);
        change = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Q"))
        {
            if (pauseController.IsPaused)
                return;
            ShowCircle();
            mousePos = Input.mousePosition;
        }

        if (Input.GetButton("Q"))
        {
            if (pauseController.IsPaused)
                HideCircle();

            if (mode == Mode.Mouse)
            {
                CalcMouse();
            }
            else if(mode == Mode.ScrollWheel)
            {
                if (Input.GetAxis("Mouse ScrollWheel") != 0)
                    CalcScroolWheel();
            }
            else
            {
                if ((mousePos - Input.mousePosition).sqrMagnitude < dist)
                    if (Input.GetAxis("Mouse ScrollWheel") != 0)
                        CalcScroolWheel();
                    else
                    {

                    }
                else
                {
                    CalcMouse();
                    mousePos = Input.mousePosition;
                }
            }
        }

        if (Input.GetButtonUp("Q"))
        {
            HideCircle();
            if(currentRealPt < commands.Length)
            {
                Debug.Log($"Result: {commands[currentRealPt].command}");
                controller.ReceiveCommand(commands[currentRealPt].command);
            }
            else
                Debug.Log($"currentRealPt:{currentRealPt}");
        }
    }

    private void CreateCircle()
    {
        segmentTitles = new GameObject[commands.Length];
        int angle = 0;
        for (int i = 0; i < NUMBER_OF_PIECES; ++i)
        {
            segments[i] = Instantiate(GreyPart, circle.transform.position, Quaternion.AngleAxis(angle, Vector3.forward), circle.transform);
            segments[i].SetActive(false);
            angle -= 45;

        }

        for(int i = 0; i < commands.Length; ++i)
        {
            segmentTitles[i] = Instantiate(commands[i].text, circle.transform);
            segmentTitles[i].SetActive(false);
        }
    }

    private void ShowCircle()
    {
        if (slowMo)
        {
            Time.timeScale = slowMoSpeed;
            currentlyInSLowMo = true;
        }

        //reset pointers
        currentScrollWheelPt = 0;
        currentCirclePt = 0;
        currentRealPt = 0;

        //show elements
        for (int i = 0; i < NUMBER_OF_PIECES; ++i)
        {
            segments[i].SetActive(true);
        }

        for (int i = 0; i < commands.Length; ++i)
        {
            segmentTitles[i].SetActive(true);
        }

        //highlight element
        Blue();
    }

    private void HideCircle()
    {
        if (slowMo)
        {
            if (pauseController.IsPaused)
                Time.timeScale = 0f;
            else
            {
                Time.timeScale = 1f;
            }
            currentlyInSLowMo = false;

        }
        Grey();

        for (int i = 0; i < NUMBER_OF_PIECES; ++i)
        {
            segments[i].SetActive(false);
        }

        for (int i = 0; i < commands.Length; ++i)
        {
            segmentTitles[i].SetActive(false);
        }
    }

    private void CalcScroolWheel()
    {
        currentScrollWheelPt -= Input.GetAxis("Mouse ScrollWheel");
        int temp = Mathf.FloorToInt(currentScrollWheelPt * sensitivity);
        if(temp > currentCirclePt)
        {
            segments[currentRealPt].GetComponent<Image>().sprite = grey;
            currentCirclePt++;
            ChangePt();
        }
        else if(temp < currentCirclePt)
        {
            segments[currentRealPt].GetComponent<Image>().sprite = grey;
            currentCirclePt--;
            ChangePt();
        }
        
    }

    private void ChangePt()
    {
        currentRealPt = currentCirclePt % NUMBER_OF_PIECES;
        if (currentRealPt > 0)
        {
            segments[currentRealPt].GetComponent<Image>().sprite = blue;
        }
        else if (currentRealPt < 0)
        {
            currentRealPt += NUMBER_OF_PIECES;
            segments[currentRealPt].GetComponent<Image>().sprite = blue;
        }
        else
            segments[currentRealPt].GetComponent<Image>().sprite = blue;

    }

    private void CalcMouse()
    {
        Vector2 vec = Input.mousePosition - circle.transform.position;
        Debug.DrawLine(Input.mousePosition, circle.transform.position, Color.red);

        float angle = Vector2.Angle(vec, Vector2.up);
        float dotProduct = Vector2.Dot(Vector2.right, vec);

        //line is on right half of circle
        if(dotProduct < 0)
        {
            angle = 360 - angle;
        }

        angle += (360 / NUMBER_OF_PIECES)/2;// angle + offset
        int pt = Mathf.FloorToInt(angle / (360 / NUMBER_OF_PIECES));

        if (pt >= NUMBER_OF_PIECES)
            pt = 0;

        if (pt != currentRealPt)
        {
            change = true;
            scalingDownPt = currentCirclePt;
            Grey();
            currentRealPt = pt;
            scalingUpPt = currentRealPt;
            Blue();
            audioSource.PlayOneShot(audioSource.clip);
        } 
    }

    private void Blue()
    {
        segments[currentRealPt].GetComponent<Image>().sprite = blue;
        segments[currentRealPt].transform.localScale = new Vector3(newScale, newScale, newScale);
        audioSource.PlayOneShot(audioSource.clip);
    }

    private void Grey()
    {
        segments[currentRealPt].GetComponent<Image>().sprite = grey;
        segments[currentRealPt].transform.localScale = Vector3.one;
    }

    public enum Mode
    {
        ScrollWheel,
        Mouse,
        Both
    }

    [System.Serializable]
    public class Commands
    {
        public GameObject text;
        public BasicCommandList command;
    }
}
