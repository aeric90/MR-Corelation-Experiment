using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Unity.VisualScripting;

public enum FITTS_STATUS
{
    INACTIVE,
    CALIBRATION,
    PRACTICE,
    TRIAL,
    POST_TRAIL,
    END
}

[System.Serializable]
public class FittsTrial
{
    public int numOfTargets = 0;
    public FittsCondition[] conditions;
}

[System.Serializable]
public class FittsCondition
{
    public float amplitude = 0.0f;
    public float width = 0.0f;
}

[System.Serializable]
public class FittsSelection
{
    public int participantID;
    public int conditionID;
    public string conditionText;
    public int targetNum;
    public float amplitude;
    public float width;
    public float time;
    public float selectionX;
    public float selectionY;
    public float selectionZ;
    public float targetX;
    public float targetY;
    public float targetZ;
}

public class FittsVRController : MonoBehaviour
{
    public static FittsVRController instance;
    public bool testing = false;
    public FITTS_STATUS currentStatus = FITTS_STATUS.INACTIVE;

    public int participantID = 0;
    public FittsTrial practiceTrials;
    public FittsTrial experimentTrials;

    public bool selectionTrial = true;
    public int conditionRepetitions = 1;

    private List<int> conditionSquareNew = new List<int>();

    public GameObject targetPrefabHigh;
    public GameObject targetPrefabLow;
    public GameObject targetPrefabSimple;

    public List<Material> targetMatsHigh = new List<Material>();
    public List<Material> targetMatsLow = new List<Material>();
    public List<Material> targetMatsSimple = new List<Material>();

    public GameObject targetPrefab;
    public Material targetBasicMaterial;
    public Material targetActiveMaterial;
    public Material targetInactiveMaterial;
    public Material targetSelectedMaterial;

    public GameObject targetContainer;

    public bool fittsRunning = false;

    private bool practiceComplete = false;
    private bool trialComplete = false;
    private int conditionID = -1;
    private string conditionText = "";
    private int currentTrial = -1;
    private int numberOfTrialsComplete = -1;
    private int currentTotalTargets = 0;
    private float currentAmplitude = 0.0f;
    private float currentTargetWidth = 1.0f;
    private float lastTargetTime = 0.0f;

    private List<GameObject> targets = new List<GameObject>();

    public int targetCount = 0;

    private int currentTargetIndex = 0;

    private StreamWriter detailOutput;
    private StreamWriter summaryOutput;

    private List<FittsSelection> selections = new List<FittsSelection>();

    private List<float> tList = new List<float>();
    private List<float> dXlist = new List<float>();

    private bool touch = false;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void SetPID(int PID)
    {
        participantID = PID;
        conditionSquareNew = LatinSquareGenerator(experimentTrials.conditions.Length, PID);
    }

    public void SetConditionID(int conditionID)
    {
        this.conditionID = conditionID;
        this.conditionText = "";
    }

    public void SetConditionID(int conditionID, string conditionText)
    {
        this.conditionID = conditionID;
        this.conditionText = conditionText;
    }

    public void StartFitts()
    {
        Debug.Log("Fitts VR - Start");
        fittsRunning = true;
        numberOfTrialsComplete = -1;
        if(currentStatus == FITTS_STATUS.POST_TRAIL) currentStatus = FITTS_STATUS.TRIAL;
        NextTrial();
    }

    public void ResetTargets()
    {
        Debug.Log("Fitts VR - Reseting Targets");
        DeleteTargets();

        for(float i = 0.0f; i < currentTotalTargets; i++)
        {
            //float x = targetContainer.transform.position.x + (currentAmplitude / 2.0f) * Mathf.Cos((Mathf.PI * 2) * (i / currentTotalTargets));
            //float y = targetContainer.transform.position.y + (currentAmplitude / 2.0f) * Mathf.Sin((Mathf.PI * 2) * (i / currentTotalTargets));
            float x = (currentAmplitude / 2.0f) * Mathf.Cos((Mathf.PI * 2) * (i / currentTotalTargets));
            float y = (currentAmplitude / 2.0f) * Mathf.Sin((Mathf.PI * 2) * (i / currentTotalTargets));
            //GameObject newTarget = Instantiate(targetPrefab, new Vector3(x, y, targetContainer.transform.localPosition.z), targetPrefab.transform.rotation, targetContainer.transform);
            GameObject newTarget = Instantiate(targetPrefab, targetContainer.transform, false);
            newTarget.transform.localPosition = new Vector3(newTarget.transform.localPosition.x + x, newTarget.transform.localPosition.y + y, newTarget.transform.localPosition.z);
            newTarget.GetComponent<MeshRenderer>().material = targetBasicMaterial;
            newTarget.transform.localScale = new Vector3(currentTargetWidth, currentTargetWidth, currentTargetWidth); 
            targets.Add(newTarget);
        }
        targetCount = 0;
        currentTargetIndex = 0;
        SetNextActiveTarget();
    }

    private void DeleteTargets()
    {
        Debug.Log("Fitts VR - Deleting Targets");
        foreach (GameObject target in targets) DestroyImmediate(target);
        targets.Clear();
    }

    private void SetNextActiveTarget()
    {
        if (targetCount > 0) {
            if (targetCount == 1)
            {
                targets[currentTargetIndex].gameObject.GetComponent<Renderer>().material = targetBasicMaterial;
            }
            else
            {
                targets[currentTargetIndex].gameObject.GetComponent<Renderer>().material = targetInactiveMaterial;
            }

            if (targetCount < currentTotalTargets)
            {
                int halfWay = (currentTotalTargets + 1) / 2;
                currentTargetIndex = (currentTargetIndex + halfWay) % currentTotalTargets;
            } else
            {
                currentTargetIndex = 0;
            }
        }

        targets[currentTargetIndex].gameObject.GetComponent<Renderer>().material = targetActiveMaterial;
    }

    public void StartCalibration()
    {
        Debug.Log("Fitts VR - Start Calibration");
        currentStatus = FITTS_STATUS.CALIBRATION;
    }

    public void StartPractice()
    {
        Debug.Log("Fitts VR - Start Practice");
        currentTotalTargets = practiceTrials.numOfTargets;
        currentStatus = FITTS_STATUS.PRACTICE;
    }

    public void StartTrials()
    {
        Debug.Log("Fitts VR - Start Trial");
        currentTotalTargets = experimentTrials.numOfTargets;
        currentStatus = FITTS_STATUS.TRIAL;

        if (!testing)
        {
            detailOutput = new StreamWriter(Application.persistentDataPath + "/FittsVR-Detail-" + DateTime.Now.ToString("ddMMyy-MMss-") + participantID + ".csv");
            detailOutput.WriteLine("PID,TID,#,A,W,ID,T,sX,sY,sZ,tX,tY,tZ,dX,dY,dZ");

            summaryOutput = new StreamWriter(Application.persistentDataPath + "/FittsVR-Summary-" + DateTime.Now.ToString("ddMMyy-MMss-") + participantID + ".csv");
            summaryOutput.WriteLine("Participant ID,Trial ID,Trial Text,A,W,ID,MT,MDx,SDx,We,IDe,TP");
        }
    }

    public void EndTrial()
    {
        Debug.Log("Fitts VR - End Trial");
        trialComplete = true;
        DeleteTargets();
        currentStatus = FITTS_STATUS.POST_TRAIL;
    }

    public void EndFitts()
    {
        Debug.Log("Fitts VR - End Fitts");
        fittsRunning = false;
        currentStatus = FITTS_STATUS.END;
        if (!testing)
        {
            detailOutput.Close();
            summaryOutput.Close();
        }
    }

    public void TargetTriggerIn(GameObject target)
    {
        if (targets[currentTargetIndex] == target)
        {
            touch = true;
            if (selectionTrial == true) target.GetComponent<Renderer>().material = targetSelectedMaterial;

        }
    }

    public void TargetTriggerOut(GameObject target)
    {
        if (targets[currentTargetIndex] == target)
        {
            touch = false;
            if (selectionTrial == true) target.GetComponent<Renderer>().material = targetActiveMaterial;
        }
    }

    public void TargetSelected(Vector3 selectionVector)
    {
        if (fittsRunning)
        {
            if (selectionTrial == false || (selectionTrial == true && touch == true))
            {
                GetComponent<AudioSource>().Play();

                switch (currentStatus)
                {
                    case FITTS_STATUS.TRIAL:
                        if (targetCount > 0)
                        {
                            AddSelection(selectionVector);
                            if (!testing) DetailOutput(selectionVector);
                        }
                        break;
                }

                lastTargetTime = Time.time;
                touch = false;
                targetCount++;

                if (targetCount > currentTotalTargets)
                {
                    NextTrial();
                }
                else
                {
                    SetNextActiveTarget();
                }
            }
        }
    }

    private void NextTrial()
    {
        FittsCondition newCondition = new FittsCondition();

        switch (currentStatus)
        {
            case FITTS_STATUS.CALIBRATION:
                currentTotalTargets = 5;
                currentAmplitude = 0.7f;
                currentTargetWidth = 0.06f;
                ResetTargets();
                break;
            case FITTS_STATUS.PRACTICE:
                Debug.Log("Fitts VR - Next Practice");
                currentTrial++;
                if (currentTrial >= practiceTrials.conditions.Length)
                {
                    practiceComplete = true;
                    currentTrial = 0;
                }
                newCondition = practiceTrials.conditions[currentTrial];
                trialComplete = false;
                currentAmplitude = newCondition.amplitude;
                currentTargetWidth = newCondition.width;
                dXlist.Clear();
                tList.Clear();
                ResetTargets();
                break;
            case FITTS_STATUS.TRIAL:
                numberOfTrialsComplete++;
                if (numberOfTrialsComplete >= conditionSquareNew.Count * conditionRepetitions)
                {
                    if(!testing) SummaryOutput();
                    EndTrial(); 
                }
                else
                {
                    Debug.Log("Fitts VR - Next Trial - " + numberOfTrialsComplete);
                    currentTrial = conditionSquareNew[numberOfTrialsComplete % conditionSquareNew.Count];
                    newCondition = experimentTrials.conditions[currentTrial];
                    trialComplete = false;
                    currentAmplitude = newCondition.amplitude;
                    currentTargetWidth = newCondition.width;
                    //dXlist.Clear();
                    //tList.Clear();
                    ResetTargets();
                }
                break;
        }
    }


    public bool GetPracticeComplete()
    {
        return practiceComplete;
    }

    public bool GetTrialComplete()
    {
        return trialComplete;
    }

    private void AddSelection(Vector3 selectionVector)
    {
        FittsSelection newSelection = new FittsSelection();

        newSelection.participantID = participantID;
        newSelection.conditionID = conditionID;
        newSelection.conditionText = conditionText;
        newSelection.targetNum = targetCount;
        newSelection.amplitude = currentAmplitude;
        newSelection.width = currentTargetWidth;
        newSelection.time = Time.time - lastTargetTime;
        newSelection.selectionX = selectionVector.x;
        newSelection.selectionY = selectionVector.y;
        newSelection.selectionZ = selectionVector.z;
        newSelection.targetX = targets[currentTargetIndex].transform.position.x;
        newSelection.targetY = targets[currentTargetIndex].transform.position.y;
        newSelection.targetZ = targets[currentTargetIndex].transform.position.z;
        
        selections.Add(newSelection);
    }

    private void DetailOutput(Vector3 selectionVector)
    {
        string outputLine = "";

        outputLine += participantID + ",";
        outputLine += conditionText + ",";
        outputLine += targetCount + ",";
        outputLine += currentAmplitude + ",";
        outputLine += currentTargetWidth + ",";
        outputLine += Math.Log((currentAmplitude / currentTargetWidth) + 1, 2) + ",";
        float selectionTime = Time.time - lastTargetTime;
        //tList.Add(selectionTime);
        outputLine += selectionTime + ",";
        outputLine += selectionVector.x + ",";
        outputLine += selectionVector.y + ",";
        outputLine += selectionVector.z + ",";
        outputLine += targets[currentTargetIndex].transform.position.x + ",";
        outputLine += targets[currentTargetIndex].transform.position.y + ",";
        outputLine += targets[currentTargetIndex].transform.position.z + ",";

        float xDelta = Math.Abs(targets[currentTargetIndex].transform.position.x - selectionVector.x);
        //dXlist.Add(xDelta);
        float yDelta = Math.Abs(targets[currentTargetIndex].transform.position.y - selectionVector.y);
        float zDelta = Math.Abs(targets[currentTargetIndex].transform.position.z - selectionVector.z);

        outputLine += xDelta + ",";
        outputLine += yDelta + ",";
        outputLine += zDelta + ",";

        detailOutput.WriteLine(outputLine);
    }

    private void SummaryOutput()
    {
        foreach (FittsCondition condition in experimentTrials.conditions)
        {
            string outputLine = "";

            outputLine += participantID + ",";
            outputLine += conditionID + ",";
            outputLine += conditionText + ",";
            outputLine += condition.amplitude + ",";
            outputLine += condition.width + ",";
            float ID = (float)Math.Log((condition.amplitude / condition.width) + 1, 2);
            outputLine += ID + ",";

            foreach (FittsSelection selection in selections)
            {
                if(selection.amplitude == condition.amplitude && selection.width == condition.width)
                {
                    tList.Add(selection.time);
                    float xDelta = Math.Abs(selection.targetX - selection.selectionX);
                    dXlist.Add(xDelta);
                }
            }

            float mT = MeanFromList(tList);
            outputLine += mT + ",";

            float mDx = MeanFromList(dXlist);
            outputLine += mDx + ",";

            float sDx = STDevFromList(dXlist, mDx);
            outputLine += sDx + ",";

            float We = sDx * 4.133f;
            outputLine += We + ",";

            float IDe = (float)Math.Log((condition.amplitude / We) + 1, 2);
            outputLine += IDe + ",";

            float TP = IDe / mT;
            outputLine += TP;

            summaryOutput.WriteLine(outputLine);

            tList.Clear();
            dXlist.Clear();
        }

        selections.Clear();
    }

    private float MeanFromList(List<float> input) 
    {
        float totalValue = 0.0f;

        foreach (float value in input) totalValue += value;

        return totalValue / input.Count;
    }

    private float STDevFromList(List<float> input, float mean)
    {
        List<float> deviations = new List<float>();

        foreach (float value in input) deviations.Add((float)Math.Pow(value - mean, 2));

        return (float)Math.Sqrt(MeanFromList(deviations));
    }

    private List<int> LatinSquareGenerator(int conditions, int participantID)
    {
        List<int> result = new List<int>();

        int j = 0;
        int h = 0;

        for(int i = 0; i < conditions; i++)
        {
            int val = 0;

            if(i < 2 || i % 2 != 0) {
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

        if(conditions % 2 != 0 && participantID % 2 != 0)
        {
            result.Reverse();
        }

        return result;
    }

    public void ChangeTargetPrefab(GameObject targetPrefab) 
    {
        this.targetPrefab = targetPrefab;
    }

    public void ChangeTargetMaterials(Material basicMat, Material activeMat, Material inactiveMat, Material selectedMat)
    {
        targetBasicMaterial = basicMat;
        targetActiveMaterial = activeMat;
        targetInactiveMaterial = inactiveMat;
        targetSelectedMaterial = selectedMat;
    }

    public void setSelectionCondition(bool status)
    {
        selectionTrial = status;
    }

    public void setTargetContainer(GameObject targetContainer)
    {
        this.targetContainer = targetContainer;
    }

    public void SetTargetMeshHigh()
    {
        ChangeTargetPrefab(targetPrefabHigh);
    }

    public void SetTargetMeshLow()
    {
        ChangeTargetPrefab(targetPrefabLow);
    }

    public void SetTargetMeshSimple()
    {
        ChangeTargetPrefab(targetPrefabSimple);
    }

    public void SetTargetMatHigh()
    {
        ChangeTargetMaterials(targetMatsHigh[0], targetMatsHigh[1], targetMatsHigh[2], targetMatsHigh[3]);
    }

    public void SetTargetMatLow()
    {
        ChangeTargetMaterials(targetMatsLow[0], targetMatsLow[1], targetMatsLow[2], targetMatsLow[3]);
    }

    public void SetTargetMatSimple()
    {
        ChangeTargetMaterials(targetMatsSimple[0], targetMatsSimple[1], targetMatsSimple[2], targetMatsSimple[3]);
    }
}
