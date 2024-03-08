using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FittsVRControllerTracker : MonoBehaviour
{
    public static FittsVRControllerTracker instance;

    public GameObject fittsTrackingPoint;

    public float timeToRecord = 0.25f;
    private float lastCheck;

    private int participantID = -1;
    private int expTrailID = -1;
    private GameObject trackedObject = null;
    private Vector3 movementTarget = Vector3.zero;
    private bool trackerOn = false;
    private bool trackerPaused = true;

    private StreamWriter output;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        lastCheck = Time.time;
    }

    public void SetPID(int PID)
    {
        participantID = PID;
    }

    public void SetTrackedObject(GameObject trackedObject)
    {
        this.trackedObject = trackedObject;
    }

    public void SetMovementTarget(Vector3 movementTarget)
    {
        this.movementTarget = movementTarget;
    }

    public void SetExpTrial(int trialID)
    {
        expTrailID = trialID;  
    }

    public void SetTrackerPaused(bool status)
    {
        trackerPaused = status;
    }

    public void SetTrackerOn(bool status)
    {
        trackerOn = status;

        if (status)
        {
            output = new StreamWriter(Application.persistentDataPath + "/FittsVR-Tracking-" + DateTime.Now.ToString("ddMMyy-MMss-") + participantID + ".csv");
            output.WriteLine("T,TID,PID,X,Y,Z,tX,tY,tZ");
        }
        else if (!status)
        {
            output.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(movementTarget == null) movementTarget = Vector3.zero;

        if(trackerOn && !trackerPaused && trackedObject != null)
        {
            fittsTrackingPoint.transform.position = trackedObject.transform.position;

            if(lastCheck - Time.deltaTime >= timeToRecord)
            {
                string outputString = "";
                outputString += Time.time + ",";
                outputString += expTrailID + ",";
                outputString += participantID + ",";
                outputString += Math.Round(fittsTrackingPoint.transform.localPosition.x, 4) + ",";
                outputString += Math.Round(fittsTrackingPoint.transform.localPosition.y, 4) + ",";
                outputString += Math.Round(fittsTrackingPoint.transform.localPosition.z, 4) + ",";
                outputString += Math.Round(movementTarget.x, 4) + ",";
                outputString += Math.Round(movementTarget.y, 4) + ",";
                outputString += Math.Round(movementTarget.z, 4) + ",";

                output.WriteLine(outputString);
            }

            lastCheck = Time.time;
        }
    }
}
