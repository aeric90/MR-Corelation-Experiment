using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartUIController : MonoBehaviour
{
    public static StartUIController instance;

    public TextMeshProUGUI roundText;
    public TextMeshProUGUI conditionText;

    public void Awake()
    {
        instance = this;
    }

    public void updateRoundText(int roomNumber, int currentRound)
    {
        roundText.text = "Room " + (roomNumber + 1) + " - Round " + currentRound;
    }

    public void updateConditionText(int conditionValue)
    {
        conditionText.text = "Selected - ";
        if (conditionValue == 0) conditionText.text += " OFF";
        if (conditionValue == 1) conditionText.text += " ON";
    }

    public void nextClick()
    {
        ExperimentController.instance.UIStartRound();
    }
}
