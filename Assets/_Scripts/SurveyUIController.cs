using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

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
    public string surveyFile;
    public TMPro.TextMeshProUGUI surveyTitle;
    public GameObject questionContainer;
    public GameObject questionPrefab;
    public TMPro.TextMeshProUGUI surveyError;
    public ScrollRect questionContent;

    private SurveyContainer surveyContainer = new SurveyContainer();

    // Start is called before the first frame update
    void Start()
    {
        if (surveyFile != null)
        {
            ImportSurvey(Application.persistentDataPath + "/surveys/" + surveyFile);
            if(surveyContainer.SurveyQuestions.Count > 0) SetupSurvey();
            LayoutRebuilder.ForceRebuildLayoutImmediate(questionContainer.GetComponent<RectTransform>());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        questionContent.verticalNormalizedPosition = 1.0f;
        if(ExperimentController.instance.GetCurrentState() == EXPERIMENT_STATE.SURVEY) ExperimentOutputController.instance.SurveyStart(surveyTitle.text);
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
        foreach(Transform questionPanel in questionContainer.transform)
        {
            questionPanel.GetComponent<SurveyQuestionController>().ResetQuestion();
        }
    }

    private void ExportSurvey(string file_name)
    {
        StreamWriter surveyOutput = new StreamWriter(file_name);
        surveyOutput.WriteLine("PID,ROOM,SURVEY,Q,A");

        int questionNumber = 0;

        foreach (Transform questionPanel in questionContainer.transform)
        {
            questionNumber++;
            string outputLine = "";

            outputLine += ExperimentController.instance.getParticipantID() + ",";
            outputLine += ExperimentController.instance.getRoomID() + ",";
            outputLine += surveyTitle.text + ",";
            outputLine += questionNumber + ",";
            outputLine += questionPanel.GetComponent<SurveyQuestionController>().answerSlider.value;

            surveyOutput.WriteLine(outputLine);
        }

        surveyOutput.Close();
    }

    public void CompleteSurvey()
    {
        int errorCount = 0;
        string errorList = "";
        int questionNumber = 0;

        foreach (Transform questionPanel in questionContainer.transform)
        {
            questionNumber++;

            if (questionPanel.GetComponent<SurveyQuestionController>().answerSlider.value == 0)
            {
                errorCount++;
                errorList += questionNumber + " ";
            }
        }
 
        if(errorCount > 0)
        {
            surveyError.text = "Questions Unanswered (" + errorList + ")";
        }
        else
        {
            surveyError.text = "";
            ExportSurvey(Application.persistentDataPath + "/" + ExperimentController.instance.getParticipantID() + "_" + ExperimentController.instance.getRoomID() + "_" + surveyTitle.text + ".csv");
            ResetSurvey();
            ExperimentOutputController.instance.SurveyEnd(surveyTitle.text);
            ExperimentController.instance.nextSurvey();
        }
    }
}
