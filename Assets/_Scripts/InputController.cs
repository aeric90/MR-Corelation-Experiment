using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private bool topButtonPress = false;
    private bool bottomButtonPress = false;
    private bool triggerDown = false;


    // Update is called once per frame
    void Update()
    {

        if ((OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Four)) && !topButtonPress)
        {
            topButtonPress = true;
            ExperimentController.instance.TopButtonPress();
        }

        if ((Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.Three)) && !triggerDown)
        {
            bottomButtonPress = true;
            ExperimentController.instance.BottomButtonPress();
        }

        if ((OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) && !triggerDown)
        {
            triggerDown = true;
            ExperimentController.instance.TriggerPress();
        }

        if ((OVRInput.GetUp(OVRInput.Button.Two) || OVRInput.GetUp(OVRInput.Button.Four)) && topButtonPress) topButtonPress = false;
        if ((OVRInput.GetUp(OVRInput.Button.One) || OVRInput.GetUp(OVRInput.Button.Three)) && bottomButtonPress) bottomButtonPress = false;
        if ((OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger)) && triggerDown) triggerDown = false;
    }
}
