using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeUIController : MonoBehaviour
{
    public GameObject nextButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (FittsVRController.instance.GetPracticeComplete() || ExperimentController.instance.testing) nextButton.SetActive(true);    
    }

    public void nextClick()
    {
        ExperimentController.instance.PracticeUINext();
    }
}
