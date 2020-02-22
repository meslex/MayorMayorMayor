using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitsController : MonoBehaviour
{
    [HideInInspector]
    public List<FriendlyUnitController> selectedSoldiers;
    [HideInInspector]
    public PauseController pause;

    public LayerMask groundLayer;
    public float radiusOfCommunicationUnitToUnit;
    public int maxSoldiersToReactToCommand;
    public Text soldiersText;
    public Text casualtiesText;
    public Text selectedSoldiersText;
    public Transform[] dutyPoints;
    public AudioClip[] commandConfirmation;

    protected int currentDutyPt;
    protected FrameSelect frameSelect;
    protected List<FriendlyUnitController> soldiers = new List<FriendlyUnitController>();
    protected AudioSource audioSource;
    protected int casualties; 
    public float NumberOfSolders { get { return soldiers.Count; } }


    private void Awake()
    {
        frameSelect = GetComponent<FrameSelect>();
        pause = GetComponent<PauseController>();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        frameSelect.allUnits = soldiers;
        if(casualtiesText != null)
            casualtiesText.text = "Casualties: 0";

        //sqrMagnitude would be used in calculations
        radiusOfCommunicationUnitToUnit = radiusOfCommunicationUnitToUnit * radiusOfCommunicationUnitToUnit;
    }

    // Update is called once per frame
    void Update()
    {
        if (pause.IsPaused)
            return;

        if(soldiersText != null)
            soldiersText.text = "Soldiers: " + NumberOfSolders;

        /*if (Input.GetKey("escape"))
        {
            Application.Quit();
        }*/

        /*if (Input.GetButtonUp("Q"))
        {
            Attack();
        }

        if (Input.GetButtonUp("P"))
        {
            Patrol();
        }

        if (Input.GetButtonUp("E"))
        {
            Duty();
        }

        if (Input.GetButtonUp("R"))
        {
            Retreat();
        }*/

        if (Input.GetButton("Ctrl") && Input.GetButtonUp("F"))
        {
            Debug.Log("F into chat");
            selectedSoldiers.Clear();
            for (int i = 0; i < soldiers.Count; ++i)
            {
                selectedSoldiers.Add(soldiers[i]);
                soldiers[i].ChangeMaterial(MaterialState.SelectedMaterial);
            }
        }

        //RTS like control
        /*if (Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 200f, groundLayer))
            {
                for (int i = 0; i < selectedSoldiers.Count; ++i)
                {
                    selectedSoldiers[i].MoveTo(hit.point, AgentState.MoveToPoint);
                }
            }

        }*/

    }

    public void ReceiveCommand(BasicCommandList command)
    {
        if (command == BasicCommandList.Attack)
            Attack();
        else if (command == BasicCommandList.Duty)
            Duty();
        else if (command == BasicCommandList.Patrol)
            Patrol();
        else if (command == BasicCommandList.Retreat)
            Retreat();
        else
            None();


    }

    public void ConfirmCommandReceving()
    {
        if (audioSource != null && selectedSoldiers.Count > 0)
            audioSource.PlayOneShot(commandConfirmation[Random.Range(0, commandConfirmation.Length)]);
    }

    public void None()
    {

    }

    public void Attack()
    {
        ConfirmCommandReceving();
        for (int i = 0; i < selectedSoldiers.Count; ++i)
        {
            if (!selectedSoldiers[i].IsDead)
                selectedSoldiers[i].Attack();
        }
    }

    public void Retreat()
    {
        ConfirmCommandReceving();
        for (int i = 0; i < selectedSoldiers.Count; ++i)
        {
            if (!selectedSoldiers[i].IsDead)
                selectedSoldiers[i].Retreat();
        }
    }

    public void Patrol()
    {
        ConfirmCommandReceving();
        for (int i = 0; i < selectedSoldiers.Count; ++i)
        {
            if (!selectedSoldiers[i].IsDead)
                selectedSoldiers[i].Patrol();
        }
    }

    public void Duty()
    {
        ConfirmCommandReceving();
        for (int i = 0; i < selectedSoldiers.Count; ++i)
        {
            if (currentDutyPt >= dutyPoints.Length)
                currentDutyPt = 0;
            if (!selectedSoldiers[i].IsDead)
                selectedSoldiers[i].Duty(dutyPoints[currentDutyPt++]);

        }
    }

    public void UnitAttacked(FriendlyUnitController unit, Target attacker)
    {
        List<FriendlyUnitController> soldierInNeatVicinity = new List<FriendlyUnitController>();
        for (int i = 0; i < soldiers.Count; ++i)
        {
            if ((unit.transform.position - soldiers[i].transform.position).sqrMagnitude < radiusOfCommunicationUnitToUnit)
            {
                soldierInNeatVicinity.Add(soldiers[i]);
            }
            else if ((unit.transform.position - soldiers[i].transform.position).sqrMagnitude == 0f)
                soldiers[i].ChangeTarget(attacker);
        }
        int num = Mathf.FloorToInt(soldierInNeatVicinity.Count / 3);
        for (int i = 0; i < num; ++i)
            soldierInNeatVicinity[i].ChangeTarget(attacker);
    }


    public void DecreaseNumberOfSoldiers(FriendlyUnitController cont)
    {
        soldiers.Remove(cont);
        casualties++;
        if(casualtiesText != null)
            casualtiesText.text = "Casualties: " + casualties;
    }

    public void IncreaseNumberOfSoldiers(FriendlyUnitController cont)
    {
        soldiers.Add(cont);
    }
}
