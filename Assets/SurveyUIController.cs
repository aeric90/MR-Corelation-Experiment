using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
[XmlRoot("survey_container")]
public class SurveyContainer
{
    private string surveyName;
    private List<SurveyQuestion> survey_questions = new List<SurveyQuestion>();

    public SurveyContainer() { }

    public string SurveyName
    {
        get { return surveyName; }
        set { surveyName = value; }
    }

    public List<SurveyQuestion> SurveyQuestions
    {
        get { return survey_questions; }
        set { survey_questions = value; }
    }
}

[XmlRoot("survey_question")]
public class SurveyQuestion
{
    private int questionNumber;
    private string questionText;
    private string questionLeft;
    private string questionMiddle;
    private string questionRight;

    public SurveyQuestion() {
        questionLeft = "";
        questionMiddle = "";
        questionRight = "";    
    }

    public int QuestionNumber
    {
        get { return questionNumber; }
        set { questionNumber = value; }
    }

    public string QuestionText
    {
        get { return questionText; }
        set { questionText = value; }
    }
    public string QuestionLeft
    {
        get { return questionLeft; }
        set { questionLeft = value; }
    }

    public string QuestionMiddle
    {
        get { return questionMiddle; }
        set { questionMiddle = value; }
    }

    public string QuestionRight
    {
        get { return questionRight; }
        set { questionRight = value; }
    }
}

public class SurveyUIController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI surveyTitle;
    public GameObject questionContainer;
    public GameObject questionPrefab;
    public TMPro.TextMeshProUGUI surveyError;

    private SurveyContainer surveyContainer = new SurveyContainer();

    // Start is called before the first frame update
    void Start()
    {
        ImportSurvey("D:\\MR Corelation Experiment\\ws.xml");
        SetupSurvey();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupSurvey()
    {
        surveyTitle.text = surveyContainer.SurveyName;
        foreach(SurveyQuestion question in surveyContainer.SurveyQuestions)
        {
            GameObject newQuestion = Instantiate(questionPrefab, questionContainer.transform);
            SurveyQuestionController newQuestionController = newQuestion.GetComponent<SurveyQuestionController>();
            newQuestionController.questionName.text = "Question " + question.QuestionNumber;
            newQuestionController.questionText.text = question.QuestionText;
            newQuestionController.questionLeft.text = question.QuestionLeft;
            newQuestionController.questionMiddle.text = question.QuestionMiddle;
            newQuestionController.questionRight.text = question.QuestionRight;
        }
    }

    private void ImportSurvey(string file_name)
    {
        XmlSerializer serializer = new XmlSerializer(surveyContainer.GetType());
        var myFileStream = new FileStream(file_name, FileMode.Open);
        surveyContainer = serializer.Deserialize(myFileStream) as SurveyContainer;
        myFileStream.Close();
    }

    public void ResetSurvey()
    {
        Debug.Log("Reseting " + questionContainer.transform.childCount + " Questions");

        foreach(Transform questionPanel in questionContainer.transform)
        {
            Debug.Log(questionPanel.gameObject.name);
            questionPanel.GetComponent<SurveyQuestionController>().ResetQuestion();
        }
    }

    private void ExportSurvey(string file_name)
    {
        XmlSerializer serializer = new XmlSerializer(surveyContainer.GetType());
        TextWriter textWriter = new StreamWriter(file_name);
        serializer.Serialize(textWriter, surveyContainer);
        textWriter.Close();
    }
}
