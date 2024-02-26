using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FileOutput : MonoBehaviour
{
    private StreamWriter output;

    // Run this function first to open the file with the filename provided. It also adds the initial csv header.
    public void CreateFile(string filename)
    {
        output = new StreamWriter(Application.persistentDataPath + "/" + filename + ".csv");
        output.WriteLine("ID,Time,...."); // CSV header. Put a header on each column
    }

    // Run this function to close the file, otherwise it will remain open.
    public void CloseFile(bool status)
    {
        output.Close();
    }

    // This function writes a pre-defined output line to the file.
    public void AddLine(string outputString)
    { 
        output.WriteLine(outputString); // Writes the provided line to the file.
    } 

    // The is an overlaoded AddLine that you can send each individual variable to to create the output line and write it to the file.
    public void AddLine(int ID, DateTime time) // Add more variables if you need them
    {
        string outputString = "";

        outputString += ID + ",";  // Add each variable in order with a comma after each except the last one.
        outputString += time;

        output.WriteLine(outputString); // Writes the line to the file.
    }

}

