using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureController : MonoBehaviour
{
    public GameObject implausibleObject;

    private Vector3 plausiblePos;
    private Quaternion plausibleRot;

    private Vector3 implausiblePos;
    private Quaternion implausibleRot;

    public FurnitureDetailController[] furnitureDetails;

    private void Start()
    {
        plausiblePos = GetComponent<Transform>().position;
        plausibleRot = GetComponent<Transform>().rotation;

        implausiblePos = implausibleObject.transform.position;
        implausibleRot = implausibleObject.transform.rotation;
    }

    public void setPlausible()
    {
        this.transform.position = plausiblePos;
        this.transform.rotation = plausibleRot;
    }

    public void setImplausible()
    {
        this.transform.position = implausiblePos;
        this.transform.rotation = implausibleRot;
    }

    public void setTextureHigh()
    {
        foreach(FurnitureDetailController controller in furnitureDetails) controller.setMaterialHigh();
    }

    public void setTextureLow()
    {
        foreach (FurnitureDetailController controller in furnitureDetails) controller.setMaterialLow();
    }

    public void setDetailHigh()
    {
        foreach (FurnitureDetailController controller in furnitureDetails) controller.setMeshHigh();
    }

    public void setDetailLow()
    {
        foreach (FurnitureDetailController controller in furnitureDetails) controller.setMeshLow();
    }
}
