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
    void Awake()
    {
        instance = this;
    }

    public void handUIOn()
    {
        disableAll();
        handUI.SetActive(!handUI.activeSelf);
    }

    public void calibrationUIOn()
    {
        disableAll();
        calibartionUI.SetActive(true);
    }

    public void participantUIOn()
    {
        disableAll();
        participantUI.SetActive(true);
    }

    public void roundUIOn()
    {
        disableAll();
        roundStartUI.SetActive(true);
    }

    public void endUIOn()
    {
        disableAll();
        endUI.SetActive(true);
    }

    public void practice1UIOn()
    {
        disableAll();
        practiceUI1.SetActive(true);
    }

    public void practice2UIOn()
    {
        disableAll();
        practiceUI2.SetActive(true);
    }

    public void wsSurveyUIOn()
    {
        disableAll();
        WSSurveyUI.SetActive(true);
    }

    public void susSurveyUIOn()
    {
        disableAll();
        SUSSurveyUI.SetActive(true);
    }

    public void ipqSurveyUIOn()
    {
        disableAll();
        IPQSurveyUI.SetActive(true);
    }

    public void disableAll()
    {
        handUI.SetActive(false);
        participantUI.SetActive(false);
        calibartionUI.SetActive(false);
        practiceUI1.SetActive(false);
        practiceUI2.SetActive(false);
        roundStartUI.SetActive(false);
        WSSurveyUI.SetActive(false);
        SUSSurveyUI.SetActive(false);
        IPQSurveyUI.SetActive(false);
        endUI.SetActive(false);
    }
}
