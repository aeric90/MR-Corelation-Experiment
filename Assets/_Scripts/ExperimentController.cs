using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using UnityEditor.XR;
using OculusSampleFramework;

public enum EXPERIMENT_STATE
{
    START,
    PARTICIPANT_ID,
    CALIBRATION,
    PRACTICE,
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

    private EXPERIMENT_STATE currentState;

    private bool triggerDown = false;

    public int maxRounds = 10;
    private int currentRound = 1;

    private StreamWriter writer;

    private int roomNumber = 0;
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

    public GameObject fittsController;

    private void Awake()
    {
        if(instance == null) instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        OVRManager.display.displayFrequency = 120.0f;
        OVRPlugin.systemDisplayFrequency = 120.0f;

        StartCoroutine(RoomAlignment());

        changeState(EXPERIMENT_STATE.CALIBRATION);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Three) && !buttonPress)
        {
            buttonPress = true;
            StartCoroutine(RoomAlignment());
        }

        if (OVRInput.GetUp(OVRInput.Button.Three) && buttonPress) buttonPress = false;

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            RoomController.instance.confirgureRoom(roomList[0]);
            FittsVRController.instance.ResetTargets();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RoomController.instance.confirgureRoom(roomList[1]);
            FittsVRController.instance.ResetTargets();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RoomController.instance.confirgureRoom(roomList[2]);
            FittsVRController.instance.ResetTargets();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            RoomController.instance.confirgureRoom(roomList[3]);
            FittsVRController.instance.ResetTargets();
        }

        switch (currentState)
        {
            case EXPERIMENT_STATE.CALIBRATION:
                CalibrationCheck();
                break;
            case EXPERIMENT_STATE.PRACTICE:
            case EXPERIMENT_STATE.ROUND:
                CheckClick();
                break;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger)) && triggerDown)
        {
            triggerDown = false;
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
            case EXPERIMENT_STATE.PARTICIPANT_ID:
                UIController.instance.ToggleHandUI();
                UIController.instance.ToggleParticipantUI();
                currentState = EXPERIMENT_STATE.PARTICIPANT_ID;
                break;
            case EXPERIMENT_STATE.CALIBRATION:
                UIController.instance.ToggleParticipantUI();
                UIController.instance.ToggleCalibrationUI();
                FittsVRController.instance.StartCalibration();
                FittsVRController.instance.StartFitts();
                currentState = EXPERIMENT_STATE.CALIBRATION;
                break;
            case EXPERIMENT_STATE.EXP_START:
                UIController.instance.ToggleCalibrationUI();
                
                //StartCoroutine(changeExpRoom());
                changeState(EXPERIMENT_STATE.ROUND_START);
                break;
            case EXPERIMENT_STATE.ROUND_START:
                RoomController.instance.confirgureRoom(15);
                UIController.instance.ToggleRoundUI();
                StartUIController.instance.updateRoundText(roomNumber, currentRound);
                currentState = EXPERIMENT_STATE.ROUND_START;
                break;
            case EXPERIMENT_STATE.ROUND:
                UIController.instance.ToggleRoundUI();
                currentState = EXPERIMENT_STATE.ROUND;
                break;
            case EXPERIMENT_STATE.EXP_END:
                closeStudyFile();
                EloController.instance.writeEloFile();
                RoomController.instance.confirgureRoom(15);
                UIController.instance.ToggleEndUI();
                currentState = EXPERIMENT_STATE.EXP_END;
                break;
        }
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

    public void HandUINext(string hand)
    {
        if(hand != "")
        {
            this.hand = hand;

            changeState(EXPERIMENT_STATE.PARTICIPANT_ID);
        }
    }

    public void PIDUINext(string PID)
    {
        if (PID != "")
        {
            participantID = int.Parse(PID);

            if(participantID == 0)
            {
                maxRounds = 3;
            }

            changeState(EXPERIMENT_STATE.CALIBRATION);
        }
    }

    public void CalibrationNext()
    {
        changeState(EXPERIMENT_STATE.PRACTICE);
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
        FittsVRController.instance.ChangeTargetPrefab(targetMeshHigh);
    }

    public void SetTargetMeshLow()
    {
        FittsVRController.instance.ChangeTargetPrefab(targetMeshLow);
    }

    public void SetTargetMatHigh()
    {
        FittsVRController.instance.ChangeTargetMaterials(targetMatsHigh[0], targetMatsHigh[1], targetMatsHigh[2], targetMatsHigh[3]);
    }

    public void SetTargetMatLow()
    {
        FittsVRController.instance.ChangeTargetMaterials(targetMatsLow[0], targetMatsLow[1], targetMatsLow[2], targetMatsLow[3]);
    }

    public void ConfirmHand()
    {
        changeState(EXPERIMENT_STATE.CALIBRATION);
    }


    private void CalibrationCheck()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) && !triggerDown)
        {
            Vector3 pokePosition = fittsController.transform.position;

            if (hand == "left")
            {
                pokePosition = leftPokeInteractor.transform.position;
            }
            else if (hand == "right")
            {
                pokePosition = rightPokeInteractor.transform.position;
            }

            Debug.Log(pokePosition);
            fittsController.transform.position = new Vector3(fittsController.transform.position.x, pokePosition.y, pokePosition.z);
            triggerDown = true;
        }
    }

    private void CheckClick()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) && !triggerDown)
        {
            if (hand == "left")
            {
                FittsVRController.instance.TargetSelected(leftPokeInteractor.transform.position);
            }
            else if (hand == "right")
            {
                FittsVRController.instance.TargetSelected(rightPokeInteractor.transform.position);
            }
            triggerDown = true;
        }
    }

    public void UICalibrationNext()
    {
        changeState(EXPERIMENT_STATE.PRACTICE);
    }
}
