using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FrameSelect : MonoBehaviour
{
    //this will prevent us from doing something when paused
    [HideInInspector]
    public PauseController pause;

    //Add all units in the scene to this array
    [HideInInspector]
    public List<FriendlyUnitController> allUnits;
    //Layer against which we need to fire rays
    public LayerMask groundLayer;
    //The selection square we draw when we drag the mouse to select units
    public RectTransform selectionSquareTrans;
    //Number of layer of selectable items
    public int friendlyLayer;
    //All currently selected units
    [System.NonSerialized]
    public List<FriendlyUnitController> selectedUnits = new List<FriendlyUnitController>();

    //We have hovered above this unit, so we can deselect it next update
    //and dont have to loop through all units
    protected FriendlyUnitController highlightThisUnit;

    //To determine if we are clicking with left mouse or holding down left mouse
    protected float delay = 0.2f;
    protected float clickTime = 0f;
    //The start and end coordinates of the square we are making
    protected Vector3 squareStartPos;
    protected Vector3 squareEndPos;
    //If it was possible to create a square
    protected bool hasCreatedSquare;
    //The selection squares 4 corner positions
    protected Vector3 TL, TR, BL, BR;

    protected UnitsController gameController;
    protected bool usable;

    private void Awake()
    {
        gameController = GetComponent<UnitsController>();
        pause = GetComponent<PauseController>();
    }


    private void Enter()
    {
        Debug.Log("Entered");
        usable = false;
    }

    private void Exit()
    {
        Debug.Log("Exited");
        usable = true;
    }


    void Start()
    {
        //Deactivate the square selection image
        selectionSquareTrans.gameObject.SetActive(false);
        gameController.selectedSoldiers = selectedUnits;

        PointEnter.OnEnter += Enter;
        PointEnter.OnExit += Exit;

        usable = true;

    }

    void Update()
    {
        if (pause.IsPaused)
            return;

        //Select one or several units by clicking or draging the mouse
        SelectUnits();

        //Highlight by hovering with mouse above a unit which is not selected
        HighlightUnit();

    }

    //Select units with click or by draging the mouse
    void SelectUnits()
    {
        //Are we clicking with left mouse or holding down left mouse
        bool isClicking = false;
        bool isHoldingDown = false;

        //Click the mouse button
        if (Input.GetMouseButtonDown(0))
        {
            clickTime = Time.time;

            //We dont yet know if we are drawing a square, but we need the first coordinate in case we do draw a square
            RaycastHit hit;
            //Fire ray from camera
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 200f, groundLayer))
            {
                //The corner position of the square
                squareStartPos = hit.point;
            }
        }
        //Release the mouse button
        if (Input.GetMouseButtonUp(0))
        {
            if (!usable)
                return;
            //here i need to exclude button presses

            if (Time.time - clickTime <= delay)
            {
                isClicking = true;

                Debug.Log("Screen Click");

            }

            //Select all units within the square if we have created a square
            if (hasCreatedSquare)
            {
                hasCreatedSquare = false;

                //Deactivate the square selection image
                selectionSquareTrans.gameObject.SetActive(false);

                //Clear the list with selected unit
                selectedUnits.Clear();
                //Debug.Log(" selectedUnits.Clear()");

                //Select the units
                for (int i = 0; i < allUnits.Count; i++)
                {
                    FriendlyUnitController currentUnit = allUnits[i];

                    //Is this unit within the square
                    if (IsWithinPolygon(currentUnit.transform.position))
                    {
                        currentUnit.ChangeMaterial(MaterialState.SelectedMaterial);

                        selectedUnits.Add(currentUnit);
                    }
                    //Otherwise deselect the unit if it's not in the square
                    else
                    {
                        currentUnit.ChangeMaterial(MaterialState.NormalMaterial);
                    }
                }
            }

        }
        //Holding down the mouse button
        if (Input.GetMouseButton(0))
        {
            //if (!usable)
            //return;
            if (Time.time - clickTime > delay)
            {
                isHoldingDown = true;
            }
        }

        //Select one unit with left mouse and deselect all units with left mouse by clicking on what's not a unit
        if (isClicking)
        {
            //Deselect all units
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                selectedUnits[i].ChangeMaterial(MaterialState.NormalMaterial);
            }

            //Clear the list with selected units
            selectedUnits.Clear();

            //Try to select a new unit
            RaycastHit hit;
            //Fire ray from camera
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 200f))
            {
                //Did we hit a friendly unit?
                if (hit.collider.gameObject.layer == friendlyLayer)
                {
                    FriendlyUnitController activeUnit = hit.collider.gameObject.GetComponent<FriendlyUnitController>();
                    //Set this unit to selected
                    activeUnit.ChangeMaterial(MaterialState.SelectedMaterial);
                    //Add it to the list of selected units, which is now just 1 unit
                    selectedUnits.Add(activeUnit);
                }
            }
        }

        //Drag the mouse to select all units within the square
        if (isHoldingDown)
        {
            //Activate the square selection image
            if (!selectionSquareTrans.gameObject.activeInHierarchy)
            {
                selectionSquareTrans.gameObject.SetActive(true);
            }

            //Get the latest coordinate of the square
            squareEndPos = Input.mousePosition;

            //Display the selection with a GUI image
            DisplaySquare();

            //Highlight the units within the selection square, but don't select the units
            if (hasCreatedSquare)
            {
                for (int i = 0; i < allUnits.Count; i++)
                {
                    FriendlyUnitController currentUnit = allUnits[i];

                    //Is this unit within the square
                    if (IsWithinPolygon(currentUnit.transform.position))
                    {
                        currentUnit.ChangeMaterial(MaterialState.HighlightMaterial);
                    }
                    //Otherwise deactivate
                    else
                    {
                        currentUnit.ChangeMaterial(MaterialState.NormalMaterial);
                    }
                }
            }
        }
    }

    //Highlight a unit when mouse is above it
    void HighlightUnit()
    {
        //Change material on the latest unit we highlighted
        if (highlightThisUnit != null)
        {
            //But make sure the unit we want to change material on is not selected
            bool isSelected = false;
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                if (selectedUnits[i] == highlightThisUnit)
                {
                    isSelected = true;
                    break;
                }
            }

            if (!isSelected)
            {
                highlightThisUnit.GetComponent<FriendlyUnitController>().ChangeMaterial(MaterialState.NormalMaterial);
            }

            highlightThisUnit = null;
        }

        //Fire a ray from the mouse position to get the unit we want to highlight
        RaycastHit hit;
        //Fire ray from camera
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 200f))
        {
            //Did we hit a friendly unit?
            if (hit.collider.gameObject.layer == friendlyLayer)
            {
                //Get the object we hit
                FriendlyUnitController currentObj = hit.collider.gameObject.GetComponent<FriendlyUnitController>();

                //Highlight this unit if it's not selected
                bool isSelected = false;
                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    if (selectedUnits[i] == currentObj)
                    {
                        isSelected = true;
                        break;
                    }
                }

                if (!isSelected)
                {
                    highlightThisUnit = currentObj;

                    highlightThisUnit.ChangeMaterial(MaterialState.HighlightMaterial);
                }
            }
        }
    }

    //Is a unit within a polygon determined by 4 corners
    bool IsWithinPolygon(Vector3 unitPos)
    {
        bool isWithinPolygon = false;

        //The polygon forms 2 triangles, so we need to check if a point is within any of the triangles
        //Triangle 1: TL - BL - TR
        if (IsWithinTriangle(unitPos, TL, BL, TR))
        {
            return true;
        }

        //Triangle 2: TR - BL - BR
        if (IsWithinTriangle(unitPos, TR, BL, BR))
        {
            return true;
        }

        return isWithinPolygon;
    }

    //Is a point within a triangle
    //From http://totologic.blogspot.se/2014/01/accurate-point-in-triangle-test.html
    bool IsWithinTriangle(Vector3 p, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        bool isWithinTriangle = false;

        //Need to set z -> y because of other coordinate system
        float denominator = ((p2.z - p3.z) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.z - p3.z));

        float a = ((p2.z - p3.z) * (p.x - p3.x) + (p3.x - p2.x) * (p.z - p3.z)) / denominator;
        float b = ((p3.z - p1.z) * (p.x - p3.x) + (p1.x - p3.x) * (p.z - p3.z)) / denominator;
        float c = 1 - a - b;

        //The point is within the triangle if 0 <= a <= 1 and 0 <= b <= 1 and 0 <= c <= 1
        if (a >= 0f && a <= 1f && b >= 0f && b <= 1f && c >= 0f && c <= 1f)
        {
            isWithinTriangle = true;
        }

        return isWithinTriangle;
    }

    //Display the selection with a GUI square
    void DisplaySquare()
    {
        //The start position of the square is in 3d space, or the first coordinate will move
        //as we move the camera which is not what we want
        Vector3 squareStartScreen = Camera.main.WorldToScreenPoint(squareStartPos);

        squareStartScreen.z = 0f;

        //Get the middle position of the square
        Vector3 middle = (squareStartScreen + squareEndPos) / 2f;

        //Set the middle position of the GUI square
        selectionSquareTrans.position = middle;

        //Change the size of the square
        float sizeX = Mathf.Abs(squareStartScreen.x - squareEndPos.x);
        float sizeY = Mathf.Abs(squareStartScreen.y - squareEndPos.y);

        //Set the size of the square
        selectionSquareTrans.sizeDelta = new Vector2(sizeX, sizeY);

        //The problem is that the corners in the 2d square is not the same as in 3d space
        //To get corners, we have to fire a ray from the screen
        //We have 2 of the corner positions, but we don't know which,  
        //so we can figure it out or fire 4 raycasts
        TL = new Vector3(middle.x - sizeX / 2f, middle.y + sizeY / 2f, 0f);
        TR = new Vector3(middle.x + sizeX / 2f, middle.y + sizeY / 2f, 0f);
        BL = new Vector3(middle.x - sizeX / 2f, middle.y - sizeY / 2f, 0f);
        BR = new Vector3(middle.x + sizeX / 2f, middle.y - sizeY / 2f, 0f);

        //From screen to world
        RaycastHit hit;
        int i = 0;
        //Fire ray from camera
        if (Physics.Raycast(Camera.main.ScreenPointToRay(TL), out hit, 200f, groundLayer))
        {
            TL = hit.point;
            i++;
        }
        if (Physics.Raycast(Camera.main.ScreenPointToRay(TR), out hit, 200f, groundLayer))
        {
            TR = hit.point;
            i++;
        }
        if (Physics.Raycast(Camera.main.ScreenPointToRay(BL), out hit, 200f, groundLayer))
        {
            BL = hit.point;
            i++;
        }
        if (Physics.Raycast(Camera.main.ScreenPointToRay(BR), out hit, 200f, groundLayer))
        {
            BR = hit.point;
            i++;
        }

        //Could we create a square?
        hasCreatedSquare = false;

        //We could find 4 points
        if (i == 4)
        {
            hasCreatedSquare = true;
        }
    }
}