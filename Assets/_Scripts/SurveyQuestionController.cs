using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurveyQuestionController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI questionName;
    public TMPro.TextMeshProUGUI questionText;
    public TMPro.TextMeshProUGUI questionLeft;
    public TMPro.TextMeshProUGUI questionMiddle;
    public TMPro.TextMeshProUGUI questionRight;
    public Slider answerSlider;
    public TMPro.TextMeshProUGUI answerText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        answerText.text = "Answer: " + answerSlider.value;
        if(answerSlider.value == 0)
        {
            answerText.color = Color.red;
        } else
        {
            answerText.color = Color.black;
        }
    }

    public void ResetQuestion()
    {
        answerSlider.value = 0;
    }
}
