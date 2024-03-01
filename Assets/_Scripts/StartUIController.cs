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
        roundText.text = "Room " + roomNumber + " - Round " + currentRound;
    }

    public void updateConditionText(string conditionText)
    {
        this.conditionText.text = conditionText;
    }

    public void nextClick()
    {
        ExperimentController.instance.UIStartRound();
    }
}
