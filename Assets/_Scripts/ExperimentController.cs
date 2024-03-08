using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using UnityEditor.XR;
using OculusSampleFramework;
using Photon.Pun;

public enum EXPERIMENT_STATE
{
    START,
    PARTICIPANT_ID,
    CALIBRATION,
    PRACTICE1,
    PRACTICE2,
    EXP_START,
    ROUND_START,
    ROUND,
    ROUND_END,
    SURVEY,
    EXP_END
}

public class ExperimentController : MonoBehaviour
{
    public static ExperimentController instance;

    public bool testing = false;

    private int participantID = 0;

    private EXPERIMENT_STATE currentState = EXPERIMENT_STATE.START;

    public int maxRounds = 10;
    private int currentRound = 1;
    private int roomNumber = 0;
    private int currentCondition = 0;
    private int currentSurvey = 0;

    public string hand = "";

    public GameObject leftController;
    public GameObject rightController;

    public GameObject leftControllerInteractor;
    public GameObject rightControllerInteractor;

    public GameObject leftPokeInteractor;
    public GameObject rightPokeInteractor;

    public GameObject leftRayInteractor;
    public GameObject rightRayInteractor;

    public GameObject fittsSpawn;
    public GameObject fittsController;
    public GameObject fittsSpawnPlane;

    public GameObject uiAnchor;
    public GameObject uiController;

    public List<int> roomList = new List<int>();

    public List<int> roomSquare = new List<int>();
    public List<int> conditionSquare = new List<int>();
    public List<int> surveySquare = new List<int>();

    private PhotonView photonView;

    private void Awake()
    {
        if(instance == null) instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (!testing)
        {
            OVRManager.display.displayFrequency = 120.0f;
            OVRPlugin.systemDisplayFrequency = 120.0f;
        }

        MRRoomController.instance.AlignRoom();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == EXPERIMENT_STATE.ROUND)
        {
            if (FittsVRController.instance.GetTrialComplete()) nextRound();
        }
    }

    public void TopButtonPress()
    {
        switch (currentState)
        {
            case EXPERIMENT_STATE.ROUND_START:
                MRRoomController.instance.AlignRoom();
                break;
        }
    }

    public void BottomButtonPress()
    {
        switch (currentState)
        {
            case EXPERIMENT_STATE.CALIBRATION:
                CalibrationCheckNew();
                break;
        }
    }

    public void TriggerPress()
    {
        switch (currentState)
        {
            case EXPERIMENT_STATE.PRACTICE1:
            case EXPERIMENT_STATE.PRACTICE2:
            case EXPERIMENT_STATE.ROUND:
                CheckClick();
                break;
        }
    }

    private void changeState(EXPERIMENT_STATE newState)
    {
        switch(newState)
        {
            case EXPERIMENT_STATE.START:
                //StartCoroutine(changeExpRoom());
                RoomController.instance.confirgureRoom(15);
                UIController.instance.handUIOn();
                currentState = EXPERIMENT_STATE.START;
                break;
            case EXPERIMENT_STATE.PARTICIPANT_ID:
                UIController.instance.participantUIOn();
                currentState = EXPERIMENT_STATE.PARTICIPANT_ID;
                break;
            case EXPERIMENT_STATE.CALIBRATION:
                UIController.instance.calibrationUIOn();
                FittsVRController.instance.StartCalibration();
                FittsVRController.instance.StartFitts();
                currentState = EXPERIMENT_STATE.CALIBRATION;
                break;
            case EXPERIMENT_STATE.PRACTICE1:
                UIController.instance.practice1UIOn();
                FittsVRController.instance.SetTargetMeshHigh();
                FittsVRController.instance.setSelectionCondition(true);
                FittsVRController.instance.StartPractice();
                FittsVRController.instance.StartFitts();
                currentState = EXPERIMENT_STATE.PRACTICE1;
                break;
            case EXPERIMENT_STATE.PRACTICE2:
                UIController.instance.practice2UIOn();
                FittsVRController.instance.SetTargetMeshHigh();
                FittsVRController.instance.setSelectionCondition(false);
                FittsVRController.instance.StartPractice();
                FittsVRController.instance.StartFitts();
                currentState = EXPERIMENT_STATE.PRACTICE2;
                break;
            case EXPERIMENT_STATE.EXP_START:
                UIController.instance.disableAll();
                FittsVRController.instance.EndTrial();
                FittsVRController.instance.SetPID(participantID);
                FittsVRController.instance.StartTrials();
                roomSquare = LatinSquareGenerator(4, participantID);
                conditionSquare = LatinSquareGenerator(2, participantID);
                surveySquare = LatinSquareGenerator(3, participantID);
                photonView.RPC("ExpStartUpdate", RpcTarget.All);
                if (FittsVRControllerTracker.instance != null) FittsVRControllerTracker.instance.SetTrackerOn(true);
                changeState(EXPERIMENT_STATE.ROUND_START);
                break;
            case EXPERIMENT_STATE.ROUND_START:
                RoomController.instance.confirgureRoom(roomList[roomSquare[roomNumber]]);
                FittsVRController.instance.setSelectionCondition(conditionSquare[currentCondition] == 1);
                FittsVRController.instance.SetConditionID(roomList[roomSquare[roomNumber]]);
                UIController.instance.roundUIOn();
                StartUIController.instance.updateRoundText(roomNumber, currentRound);
                StartUIController.instance.updateConditionText(conditionSquare[currentCondition]);
                photonView.RPC("RoundUpdate", RpcTarget.All, currentRound);
                photonView.RPC("RoomUpdate", RpcTarget.All, getRoomID());
                if (FittsVRControllerTracker.instance != null) FittsVRControllerTracker.instance.SetExpTrial(currentRound);
                currentState = EXPERIMENT_STATE.ROUND_START;
                break;
            case EXPERIMENT_STATE.ROUND:
                UIController.instance.disableAll();
                FittsVRController.instance.StartFitts();
                if (FittsVRControllerTracker.instance != null) FittsVRControllerTracker.instance.SetTrackerPaused(false);
                currentState = EXPERIMENT_STATE.ROUND;
                break;
            case EXPERIMENT_STATE.ROUND_END:
                FittsVRController.instance.EndTrial();
                if (FittsVRControllerTracker.instance != null) FittsVRControllerTracker.instance.SetTrackerPaused(true);
                nextRound();
                currentState = EXPERIMENT_STATE.ROUND_END;
                break;
            case EXPERIMENT_STATE.SURVEY:
                currentState = EXPERIMENT_STATE.SURVEY;
                currentSurvey = 0;
                toggleSurvey();
                break;
            case EXPERIMENT_STATE.EXP_END:
                RoomController.instance.confirgureRoom(15);
                UIController.instance.endUIOn();
                if (FittsVRControllerTracker.instance != null) FittsVRControllerTracker.instance.SetTrackerOn(false);
                photonView.RPC("ExpEndUpdate", RpcTarget.All);
                currentState = EXPERIMENT_STATE.EXP_END;
                break;
        }
    }

    public int getParticipantID()
    {
        return participantID;
    }

    public int getRoomID()
    {
        return roomList[roomSquare[roomNumber]];
    }

    public void nextRound()
    {
        if (hand == "left")
        {
            leftRayInteractor.SetActive(true);
        }
        else if (hand == "right")
        {
            rightRayInteractor.SetActive(true);
        }

        currentRound++;
        currentCondition++;

        if (currentCondition > 1 || !testing)
        {
            changeState(EXPERIMENT_STATE.SURVEY);
        } 
        else
        {
            changeState(EXPERIMENT_STATE.ROUND_START);
        }
    }

    public void nextRoom()
    {
        roomNumber++;
        currentCondition = 0;

        if (roomNumber > 3)
            changeState(EXPERIMENT_STATE.EXP_END);
        else
            changeState(EXPERIMENT_STATE.ROUND_START);
    }

    public void HandUINext(string hand)
    {
        if(hand != "")
        {
            this.hand = hand;

            if (hand == "left")
            {
                rightController.SetActive(false);
                rightControllerInteractor.SetActive(false);
                if (FittsVRControllerTracker.instance != null) FittsVRControllerTracker.instance.SetTrackedObject(leftPokeInteractor);
            }
            else if (hand == "right")
            {
                leftController.SetActive(false);
                leftControllerInteractor.SetActive(false);
                if (FittsVRControllerTracker.instance != null) FittsVRControllerTracker.instance.SetTrackedObject(rightPokeInteractor);
            }

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

            photonView.RPC("PIDUpdate", RpcTarget.All, participantID);
            if (FittsVRControllerTracker.instance != null) FittsVRControllerTracker.instance.SetPID(participantID);
            changeState(EXPERIMENT_STATE.CALIBRATION);
        }
    }

    public void PracticeUINext()
    {
        if (currentState == EXPERIMENT_STATE.PRACTICE1) changeState(EXPERIMENT_STATE.PRACTICE2);
        else if (currentState == EXPERIMENT_STATE.PRACTICE2) changeState(EXPERIMENT_STATE.EXP_START);
    }

    public void UIStartRound()
    {
        if (hand == "left")
        {
            leftRayInteractor.SetActive(false); 
        }
        else if (hand == "right")
        {
            rightRayInteractor.SetActive(false);
        }
        changeState(EXPERIMENT_STATE.ROUND); 
    }

    public void ConfirmHand()
    {
        changeState(EXPERIMENT_STATE.CALIBRATION);
    }


    private void CalibrationCheck()
    {
        Vector3 pokePosition = fittsSpawn.transform.position;

        if (hand == "left")
        {
            pokePosition = leftPokeInteractor.transform.position;
        }
        else if (hand == "right")
        {
            pokePosition = rightPokeInteractor.transform.position;
        }

        fittsSpawn = GameObject.FindGameObjectWithTag("fittsSpawn");
        //fittsSpawn.transform.localPosition = new Vector3(0.0f, pokePosition.y, pokePosition.z);
        Vector3 projection = Vector3.Project(pokePosition, fittsSpawn.transform.position);
        fittsSpawn.transform.position = new Vector3(projection.x, pokePosition.y, projection.z);
        SetFittsSpawnPoint();
    }

    private void CalibrationCheckNew()
    {
        Vector3 pokePosition = fittsSpawn.transform.position;

        if (hand == "left")
        {
            pokePosition = leftPokeInteractor.transform.position;
        }
        else if (hand == "right")
        {
            pokePosition = rightPokeInteractor.transform.position;
        }

        if(fittsSpawnPlane == null) fittsSpawnPlane = GameObject.FindGameObjectWithTag("fittsSpawnPlane");
        //fittsSpawn.transform.localPosition = new Vector3(0.0f, pokePosition.y, pokePosition.z);

        Vector3 fittsSpawnPlaneNormal = fittsSpawnPlane.transform.up;
        Debug.Log(fittsSpawnPlaneNormal);
        Vector3 projection = Vector3.ProjectOnPlane(pokePosition, fittsSpawnPlaneNormal);
        fittsSpawn.transform.position = projection;
        fittsSpawn.transform.localPosition = new Vector3(0.0f, fittsSpawn.transform.localPosition.y, fittsSpawn.transform.localPosition.z);
        SetFittsSpawnPoint();
    }

    private void CheckClick()
    {
        if (hand == "left")
        {
            FittsVRController.instance.TargetSelected(leftPokeInteractor.transform.position);
        }
        else if (hand == "right")
        {
            FittsVRController.instance.TargetSelected(rightPokeInteractor.transform.position);
        }
    }

    public void UICalibrationNext()
    {
        changeState(EXPERIMENT_STATE.PRACTICE1);
    }

    public void ResetCurrentState()
    {
        changeState(currentState);
    }

    public void SetFittsSpawnPoint()
    {
        fittsSpawn = GameObject.FindGameObjectWithTag("fittsSpawn");
        fittsController.transform.position = fittsSpawn.transform.position;
        fittsController.transform.rotation = fittsSpawn.transform.rotation;
        
    }

    public void SetUIAnchorPoint()
    {
        uiAnchor = GameObject.FindGameObjectWithTag("uiAnchor");
        uiController.transform.position = uiAnchor.transform.position;
        uiController.transform.rotation = uiAnchor.transform.rotation;
    }

    public void nextSurvey()
    {
        currentSurvey++;
        if (currentSurvey > 2)
        {
            nextRoom();
        } else
        {
            toggleSurvey();
        }
    }

    private void toggleSurvey()
    {
        switch (surveySquare[currentSurvey])
        {
            case 0:
                UIController.instance.wsSurveyUIOn();
                break;
            case 1:
                UIController.instance.susSurveyUIOn();
                break;
            case 2:
                UIController.instance.ipqSurveyUIOn();
                break;
        }
    }

    private List<int> LatinSquareGenerator(int conditions, int participantID)
    {
        List<int> result = new List<int>();

        int j = 0;
        int h = 0;

        for (int i = 0; i < conditions; i++)
        {
            int val = 0;

            if (i < 2 || i % 2 != 0)
            {
                val = j++;
            }
            else
            {
                val = conditions - h - 1;
                h++;
            }

            int idx = (val + participantID) % conditions;
            result.Add(idx);
        }

        if (conditions % 2 != 0 && participantID % 2 != 0)
        {
            result.Reverse();
        }

        return result;
    }

    [PunRPC]
    public void ExpStartUpdate()
    {
        Debug.Log("Network Exerpiment Started");
    }

    [PunRPC]
    public void PIDUpdate(int PID)
    {
        Debug.Log("Exerpiment PID Update - " + PID);
    }


    [PunRPC]
    public void RoomUpdate(int roomID)
    {
        Debug.Log("Network Room Update - " + roomID);
    }

    [PunRPC]
    public void RoundUpdate(int roundNo)
    {
        Debug.Log("Network Round Update - " + roundNo);
    }

    [PunRPC]
    public void ExpEndUpdate()
    {
        Debug.Log("Exerpiment Ended");
    }
}
