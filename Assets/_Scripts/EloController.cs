using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EloRoom
{
    private int roomID;
    private double qr;
    private int win;
    private int loss;

    public EloRoom()
    {
        roomID = -1;
        qr = 1200;
        win = 0;
        loss = 0;
    }

    public EloRoom(int roomID, double qr, int win, int loss)
    {
        this.roomID = roomID;
        this.qr = qr;
        this.win = win;
        this.loss = loss;
    }

    public double QR
    {
        get { return qr; } 
        set { qr = value; }
    }

    public int Win
    {
        get { return win; }
        set { win = value; }
    }

    public int Loss
    {
        get { return loss; }
        set { loss = value; }
    }

    public void printRoom()
    {
        Debug.Log("Room " + roomID + ": " + qr + ", " + win + ", " + loss);
    }

    public string getEloFileLine()
    {
        return roomID+","+qr+","+win+","+loss;
    }
}

public class EloController : MonoBehaviour
{
    public static EloController instance;

    private List<EloRoom> roomList = new List<EloRoom>();

    private EloRoom roomA;
    private EloRoom roomB;

    private double roomAexpected;
    private double roomBexpected;

    private int kFactor = 32;

    private bool eloFileSaved = false;

    private void Awake()
    {
        instance = this;
        //openEloFile();
    }

    private void OnDestroy()
    {
        if(!eloFileSaved) writeEloFile();
        //roomList.Clear();
    }

    public void setUpMatchUp(int roomA, int roomB)
    {
        selectRoomA(roomA);
        selectRoomB(roomB);
        calcExpected();
    }

    private void selectRoomA(int index)
    {
        roomA = roomList[index];
    }

    private void selectRoomB(int index)
    {
        roomB = roomList[index];
    }

    private void calcExpected()
    {
        double qA = Math.Pow(10, roomA.QR / 400);
        double qB = Math.Pow(10, roomB.QR / 400);

        roomAexpected = qA / (qA + qB);
        roomBexpected = qB / (qA + qB);
}

    public void roomAWin()
    {
        roomA.Win++;
        roomB.Loss++;

        double roomAchange = kFactor * (1 - roomAexpected);
        double roomBchange = kFactor * (0 - roomBexpected);

        roomA.QR += roomAchange;
        roomB.QR += roomBchange;
    }

    public void roomBWin()
    {
        roomB.Win++;
        roomA.Loss++;

        double roomAchange = kFactor * (0 - roomAexpected);
        double roomBchange = kFactor * (1 - roomBexpected);

        roomA.QR += roomAchange;
        roomB.QR += roomBchange;
    }

    public void openEloFile()
    {
        StreamReader reader;

        string inputFilename = "/qr_elo_file_";

        int pID = ExperimentController.instance.getParticipantID();

        if(pID == 0)
        {
            inputFilename += "0";
        } else
        {
            inputFilename += (pID - 1);
        }

        reader = new StreamReader(Application.persistentDataPath + inputFilename + ".csv");

        reader.ReadLine();

        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            string[] values = line.Split(",");

            EloRoom roomTemp = new EloRoom(int.Parse(values[0]), double.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));

            roomList.Add(roomTemp);
        }

        reader.Close();
    }

    public void writeEloFile()
    {
        string outputFilename = "/qr_elo_file_";

        int pID = ExperimentController.instance.getParticipantID();

        if(pID == 0)
        {
            outputFilename += "test";
        } else
        {
            outputFilename += pID;
        }

        StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/" + outputFilename + ".csv");

        writer.WriteLine("room_id,qr,w,l");

        foreach (EloRoom room in roomList) writer.WriteLine(room.getEloFileLine());

        writer.Close();
        eloFileSaved = true;
    }
}
