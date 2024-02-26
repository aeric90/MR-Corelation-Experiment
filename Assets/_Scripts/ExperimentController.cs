using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using UnityEditor.XR;

public enum EXPERIMENT_STATE
{
    START,
    CALIBRATION,
    EXP_START,
    ROUND_START,
    ROUND,
    ROUND_END,
    EXP_END
}

public class ExperimentController : MonoBehaviour
{
    public static ExperimentController instance;

    private int participantID = 0;

    public List<GameObject> leftUIs = new List<GameObject>();
    public List<GameObject> rightUIs = new List<GameObject>();

    private EXPERIMENT_STATE currentState;

    public int maxRounds = 10;
    private int currentRound = 1;

    private StreamWriter writer;

    public GameObject roomPrefab;
    private GameObject room;

    private bool studyFileOpen = false;

    private bool buttonPress = false;

    public GameObject targetMeshHigh;
    public GameObject targetMeshLow;

    public List<Material> targetMatsHigh = new List<Material>();
    public List<Material> targetMatsLow = new List<Material>();

    public string hand = "";

    public GameObject leftPokeInteractor;
    public GameObject rightPokeInteractor;

    public List<int> roomList = new List<int>();

    private void Awake()
    {
        if(instance == null) instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //OVRManager.display.displayFrequency = 120.0f;
        //OVRPlugin.systemDisplayFrequency = 120.0f;

        StartCoroutine(RoomAlignment());

        changeState(EXPERIMENT_STATE.START);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != EXPERIMENT_STATE.EXP_END)
        {
            if (OVRInput.GetDown(OVRInput.Button.Three) && !buttonPress)
            {
                buttonPress = true;
                StartCoroutine(RoomAlignment());
            }
        }

        if(currentState == EXPERIMENT_STATE.CALIBRATION)
        {

        }

        if (OVRInput.GetUp(OVRInput.Button.Three) && buttonPress) buttonPress = false;

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            RoomController.instance.confirgureRoom(roomList[0]);
            FittsVRController.fittsVRinstance.ResetTargets();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RoomController.instance.confirgureRoom(roomList[1]);
            FittsVRController.fittsVRinstance.ResetTargets();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RoomController.instance.confirgureRoom(roomList[2]);
            FittsVRController.fittsVRinstance.ResetTargets();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            RoomController.instance.confirgureRoom(roomList[3]);
            FittsVRController.fittsVRinstance.ResetTargets();
        }
    }

    private void OnDestroy()
    {
        if(studyFileOpen) closeStudyFile();
    }

    private void changeState(EXPERIMENT_STATE newState)
    {
        switch(newState)
        {
            case EXPERIMENT_STATE.START:
                StartCoroutine(changeExpRoom());
                RoomController.instance.confirgureRoom(15);
                currentState = EXPERIMENT_STATE.START;
                break;
            case EXPERIMENT_STATE.CALIBRATION:
                ToggleHandUI();
                ToggleCalibrationUI();
                currentState = EXPERIMENT_STATE.CALIBRATION;
                break;
            case EXPERIMENT_STATE.EXP_START:
                //openStudyFile();
                
                //StartCoroutine(changeExpRoom());
                changeState(EXPERIMENT_STATE.ROUND_START);
                break;
            case EXPERIMENT_STATE.ROUND_START:
                RoomController.instance.confirgureRoom(15);
                ToggleRoundUI();
                StartUIController.instance.updateRoundText(currentRound);
                currentState = EXPERIMENT_STATE.ROUND_START;
                break;
            case EXPERIMENT_STATE.ROUND:
                ToggleRoundUI();
                currentState = EXPERIMENT_STATE.ROUND;
                break;
            case EXPERIMENT_STATE.EXP_END:
                closeStudyFile();
                EloController.instance.writeEloFile();
                RoomController.instance.confirgureRoom(15);
                ToggleEndUI();
                currentState = EXPERIMENT_STATE.EXP_END;
                break;
        }
    }

    public void setParticipantID(int participantID)
    {
        this.participantID = participantID; 
    }

    public int getParticipantID()
    {
        return participantID;
    }

    public void nextRound()
    {
        currentRound++;

        if (currentRound <= maxRounds)
        {
            changeState(EXPERIMENT_STATE.ROUND_START);
        }
        else
        {
            changeState(EXPERIMENT_STATE.EXP_END);
        }
    }

    IEnumerator changeExpRoom()
    {
        RoomController.instance.setForExperiment();
        yield return new WaitForSeconds(2.0f);
    }

    private void openStudyFile()
    {
        /*
        writer = new StreamWriter(Application.persistentDataPath + "/qr_detail_" + participantID + ".csv");

        writer.WriteLine("PID,Round,Matchup,Room A,Room B,Winner,Swaps,Time");
        */
        studyFileOpen = true;
    }

    private void closeStudyFile()
    {
        /*
        writer.Close();
        */
        studyFileOpen = false;
    }


    private void updateStudyFile()
    {
        string output = "";

        writer.WriteLine(output);
    }

    public void UIStart(string PID)
    {
        if (PID != "")
        {
            participantID = int.Parse(PID);

            if(participantID == 0)
            {
                maxRounds = 3;
            }
            ToggleParticipantUI();
            changeState(EXPERIMENT_STATE.EXP_START);
        }
    }

    public void UIStartRound()
    {
        changeState(EXPERIMENT_STATE.ROUND); 
    }

    IEnumerator RoomAlignment()
    {
        yield return new WaitForSeconds(2.0f);

        GameObject floor = null;

        OVRSemanticClassification[] anchorLists = FindObjectsOfType(typeof(OVRSemanticClassification)) as OVRSemanticClassification[];

        foreach(OVRSemanticClassification anchor in anchorLists)
        {
            if(anchor.Contains("FLOOR"))
            {
                floor = anchor.gameObject;
                break;
            }
        }

        if(floor != null)
        {
            if(room !=  null) Destroy(room.gameObject);
            room = Instantiate(roomPrefab);
            room.transform.position = floor.transform.position;

            float floorZRoatation = floor.transform.eulerAngles.z + floor.transform.eulerAngles.y + 90.0f;
            room.transform.eulerAngles = new Vector3(0.0f, floorZRoatation, 0.0f);
        }

        yield return new WaitForSeconds(0.5f);
    }

    public void SetTargetMeshHigh()
    {
        FittsVRController.fittsVRinstance.ChangeTargetPrefab(targetMeshHigh);
    }

    public void SetTargetMeshLow()
    {
        FittsVRController.fittsVRinstance.ChangeTargetPrefab(targetMeshLow);
    }

    public void SetTargetMatHigh()
    {
        FittsVRController.fittsVRinstance.ChangeTargetMaterials(targetMatsHigh[0], targetMatsHigh[1], targetMatsHigh[2], targetMatsHigh[3]);
    }

    public void SetTargetMatLow()
    {
        FittsVRController.fittsVRinstance.ChangeTargetMaterials(targetMatsLow[0], targetMatsLow[1], targetMatsLow[2], targetMatsLow[3]);
    }

    public void SetHand(string hand)
    {
        this.hand = hand;
    }

    public void ConfirmHand()
    {
        changeState(EXPERIMENT_STATE.CALIBRATION);
    }

    private void ToggleHandUI()
    {
        leftUIs[0].SetActive(!leftUIs[0].activeSelf);
        rightUIs[0].SetActive(!rightUIs[0].activeSelf);
    }

    private void ToggleCalibrationUI()
    {
        if(hand == "L") leftUIs[1].SetActive(!leftUIs[0].activeSelf);
        if(hand == "R") rightUIs[1].SetActive(!rightUIs[0].activeSelf);
    }

    private void ToggleParticipantUI()
    {
        if (hand == "L") leftUIs[2].SetActive(!leftUIs[0].activeSelf);
        if (hand == "R") rightUIs[2].SetActive(!rightUIs[0].activeSelf);
    }

    private void ToggleRoundUI()
    {
        if (hand == "L") leftUIs[3].SetActive(!leftUIs[0].activeSelf);
        if (hand == "R") rightUIs[3].SetActive(!rightUIs[0].activeSelf);
    }

    private void ToggleEndUI()
    {
        if (hand == "L") leftUIs[4].SetActive(!leftUIs[0].activeSelf);
        if (hand == "R") rightUIs[4].SetActive(!rightUIs[0].activeSelf);
    }
}
