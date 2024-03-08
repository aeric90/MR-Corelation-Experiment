using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ExperimentOutputController : MonoBehaviour
{
    public static ExperimentOutputController instance;

    private StreamWriter experimentOutput;

    private int PID;
    private int RoomID = 0;
    private int RoundNo = 0;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        experimentOutput = new StreamWriter(Application.persistentDataPath + "/Exp-Data-" + DateTime.Now.ToString("ddMMyy-MMss-") + PID + ".csv");
        experimentOutput.WriteLine("PID,Room,Round,Event,T");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OutputEvent(string eventType) 
    {
        string outputLine = "";

        outputLine += PID + ",";
        outputLine += RoomID + ",";
        outputLine += RoundNo + ",";
        outputLine += eventType + ",";
        outputLine += Time.time;

        experimentOutput.WriteLine(outputLine);
    }

    public void SetPID(int PID)
    {
        this.PID = PID;
    }

    public void RoundStart(int RoundNo)
    {
        this.RoundNo = RoundNo;
        OutputEvent("ROUND_START");
    }

    public void RoundEnd()
    {
        OutputEvent("ROUND_END");
    }

    public void RoomStart(int RoomID)
    {
        if (this.RoomID != RoomID)
        {
            this.RoomID = RoomID;
            OutputEvent("ROOM_START");
        }
    }

    public void RoomEnd()
    {
        OutputEvent("ROOM_END");
    }

    public void SurveyStart(string surveyName)
    {
        OutputEvent("SURVEY_START_" +  surveyName);
    }

    public void SurveyEnd(string surveyName)
    {
        OutputEvent("SURVEY_END_" + surveyName);
    }

    public void CloseExperimentOutput()
    {
        experimentOutput.Close();
    }
}
