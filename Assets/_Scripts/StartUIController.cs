using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartUIController : MonoBehaviour
{
    public static StartUIController instance;

    public TextMeshProUGUI roundText;

    public void Awake()
    {
        instance = this;
    }

    public void updateRoundText(int currentRound)
    {
        roundText.text = "Round " + currentRound;
    }
}
