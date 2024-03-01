using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibartionUIController : MonoBehaviour
{
    public void nextClick()
    {
        ExperimentController.instance.UICalibrationNext();
    }
}
