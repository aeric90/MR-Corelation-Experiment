using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public GameObject handUI;
    public GameObject participantUI;
    public GameObject calibartionUI;
    public GameObject practiceUI1;
    public GameObject practiceUI2;
    public GameObject roundStartUI;
    public GameObject WSSurveyUI;
    public GameObject SUSSurveyUI;
    public GameObject IPQSurveyUI;
    public GameObject endUI;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void ToggleHandUI()
    {
        handUI.SetActive(!handUI.activeSelf);
    }

    public void ToggleCalibrationUI()
    {
        calibartionUI.SetActive(!calibartionUI.activeSelf);
    }

    public void ToggleParticipantUI()
    {
        participantUI.SetActive(!participantUI.activeSelf);
    }

    public void ToggleRoundUI()
    {
        roundStartUI.SetActive(!roundStartUI.activeSelf);
    }

    public void ToggleEndUI()
    {
        endUI.SetActive(!endUI.activeSelf);
    }
}
