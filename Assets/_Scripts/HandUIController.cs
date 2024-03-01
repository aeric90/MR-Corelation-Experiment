using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandUIController : MonoBehaviour
{
    public Button leftButton;
    public Button rightButton;

    private string hand = "";

    // Update is called once per frame
    void Update()
    {
        if (hand == "left")
        {
            leftButton.GetComponent<Image>().color = Color.yellow;
            rightButton.GetComponent<Image>().color = Color.white;
        }

        if(hand == "right")
        {
            leftButton.GetComponent<Image>().color = Color.white;
            rightButton.GetComponent<Image>().color = Color.yellow;
        }
    }

    public void nextClick()
    {
        ExperimentController.instance.HandUINext(hand);
    }

    public void handButton(string hand)
    {
        this.hand = hand;
    }
}
