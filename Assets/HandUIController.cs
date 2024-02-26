using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandUIController : MonoBehaviour
{
    public Button leftButton;
    public Button rightButton;

    // Update is called once per frame
    void Update()
    {
        if (ExperimentController.instance.hand == "L")
        {
            leftButton.GetComponent<Image>().color = Color.yellow;
            rightButton.GetComponent<Image>().color = Color.white;
        }

        if(ExperimentController.instance.hand == "R")
        {
            leftButton.GetComponent<Image>().color = Color.white;
            rightButton.GetComponent<Image>().color = Color.yellow;
        }
    }
}
