using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public static RoomController instance;

    public GameObject demo;
    public GameObject furnitue;

    public List<Light> lights = new List<Light>();


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void confirgureRoom(int roomID)
    {
        Debug.Log("Configuring Room " + roomID);
        string roomMask = Convert.ToString(roomID, 2).PadLeft(4, '0');

        Debug.Log(roomID + ": " + roomMask[0] + ", " + roomMask[1] + ", " + roomMask[2] + ", " + roomMask[3]);

        detailControl(roomMask[0] == '1');
        textureControl(roomMask[1] == '1');
        shadowControl(roomMask[2] == '1');
        plausibilityControl(roomMask[3] == '1');
    }

    private void detailControl(bool status)
    {
        if (status)
        {
            ExperimentController.instance.SetTargetMeshHigh();
        } 
        else
        {
            ExperimentController.instance.SetTargetMeshLow();
        }

        foreach(GameObject piece in GameObject.FindGameObjectsWithTag("furniture"))
        {
            if (status)
            {
                piece.GetComponent<FurnitureController>().setDetailHigh();
            }
            else
            {
                piece.GetComponent<FurnitureController>().setDetailLow();
            }
        }
    }

    private void textureControl(bool status)
    {
        if (status)
        {
            ExperimentController.instance.SetTargetMatHigh();
        }
        else
        {
            ExperimentController.instance.SetTargetMatLow();
        }

        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("furniture"))
        {
            if (status)
            {
                piece.GetComponent<FurnitureController>().setTextureHigh();
            }
            else
            {
                piece.GetComponent<FurnitureController>().setTextureLow();
            }
        }
    }

    public void shadowControl(bool status)
    {
        foreach(Light light in lights)
        {
            if(status)
            {
                light.shadows = LightShadows.Hard;
            }  else
            {
                light.shadows = LightShadows.None;
            }
        }
    }

    private void plausibilityControl(bool status)
    {
        foreach (GameObject piece in GameObject.FindGameObjectsWithTag("furniture"))
        {
            if (status)
            {
                piece.GetComponent<FurnitureController>().setPlausible();
            }
            else
            {
                piece.GetComponent<FurnitureController>().setImplausible();
            }
        }
    }

    public void setForDemo()
    {
        demo.SetActive(true);
        furnitue.SetActive(false);
    }

    public void setForExperiment()
    {
        demo.SetActive(false);
        furnitue.SetActive(true);
    }
}
