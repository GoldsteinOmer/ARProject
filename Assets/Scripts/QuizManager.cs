using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    [Header("Question Data")]
    public List<QnA> QnA;
    public GameObject[] options;
    public int currentQuestion;

    [Header("Panels")]
    public GameObject StartPanel;
    public GameObject OptionsPanel;
    public GameObject QuizPanel;
    public GameObject GameOverPanel;

    [Header("UI Elements")]
    public TextMeshProUGUI QuestionTxt;
    public TextMeshProUGUI ScoreTxt;
    public TextMeshProUGUI FinalScoreTxt;

    [Header("Feedback Settings")]
    public GameObject FeedbackPanel; 
    public TextMeshProUGUI FeedbackTxt; 
    public float delayDuration = 1.0f;

    [Header("Visual Components")]
    public RawImage DisplayImage;
    public Material colorBlindMat;

    private int score;
    private int questionsAnswered;
    private bool isProcessing = false;

    private void Start()
    {
        if (colorBlindMat != null) colorBlindMat.SetFloat("_Mode", 0);
        
        StartPanel.SetActive(true);
        OptionsPanel.SetActive(false);
        QuizPanel.SetActive(false);
        GameOverPanel.SetActive(false);
        
        // FIX: Ensure both are hidden and empty when the game opens
        if (FeedbackPanel != null) FeedbackPanel.SetActive(false);
        if (FeedbackTxt != null) FeedbackTxt.text = "";
    }

    #region Navigation
    public void OpenOptions() { StartPanel.SetActive(false); OptionsPanel.SetActive(true); }
    public void CloseOptions() { OptionsPanel.SetActive(false); StartPanel.SetActive(true); }

    public void StartQuiz()
    {
        StartPanel.SetActive(false);
        OptionsPanel.SetActive(false);
        QuizPanel.SetActive(true);
        score = 0;
        questionsAnswered = 0;
        UpdateScoreText();
        
        // FIX: Ensure feedback is cleared if restarting the quiz
        if (FeedbackPanel != null) FeedbackPanel.SetActive(false);
        if (FeedbackTxt != null) FeedbackTxt.text = "";

        generateQuestion();
    }

    public void MainMenu() 
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }
    #endregion

    #region Colorblind Mode Settings
    public void SetNormal(bool isOn) { if(isOn) SetMode(0); }
    public void SetDeuteranopia(bool isOn) { if(isOn) SetMode(1); }
    public void SetProtanopia(bool isOn) { if(isOn) SetMode(2); }
    public void SetTritanopia(bool isOn) { if(isOn) SetMode(3); }

    private void SetMode(int mode)
    {
        if (colorBlindMat != null)
        {
            colorBlindMat.SetFloat("_Mode", (float)mode);
        }
    }
    #endregion

    #region Quiz Logic
    public void correct()
    {
        if (isProcessing) return;
        score++;
        StartCoroutine(HandleFeedback(true));
    }

    public void wrong()
    {
        if (isProcessing) return;
        StartCoroutine(HandleFeedback(false));
    }

    IEnumerator HandleFeedback(bool isCorrect)
    {
        isProcessing = true;
        questionsAnswered++;
        UpdateScoreText();

        if (FeedbackPanel != null && FeedbackTxt != null)
        {
            FeedbackTxt.text = isCorrect ? "Correct!" : "Wrong Answer!";
            FeedbackTxt.color = isCorrect ? new Color(0f, 0.6f, 0f) : Color.red;
            FeedbackPanel.SetActive(true);
        }

        yield return new WaitForSeconds(delayDuration);

        // FIX: Clear the text content BEFORE hiding the panel
        if (FeedbackTxt != null) FeedbackTxt.text = "";
        
        if (FeedbackPanel != null) 
        {
            FeedbackPanel.SetActive(false);
        }
        
        if (QnA.Count > 0)
        {
            QnA.RemoveAt(currentQuestion);
        }
        
        generateQuestion();
        isProcessing = false;
    }

    void UpdateScoreText() 
    { 
        ScoreTxt.text = "Score: " + score + " / " + questionsAnswered; 
    }

    void SetAnswers()
    {
        for (int i = 0; i < options.Length; i++)
        {
            AnswersScript answer = options[i].GetComponent<AnswersScript>();
            answer.isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = QnA[currentQuestion].Answers[i];
            if (QnA[currentQuestion].CorrectAnswer == i + 1) answer.isCorrect = true;
        }
    }

    void generateQuestion()
    {
        if (QnA.Count > 0)
        {
            currentQuestion = Random.Range(0, QnA.Count);
            QuestionTxt.text = QnA[currentQuestion].Question;
            
            if (QnA[currentQuestion].QuestionImage != null)
            {
                DisplayImage.texture = QnA[currentQuestion].QuestionImage;
                DisplayImage.material = colorBlindMat;
                DisplayImage.gameObject.SetActive(true);
            }
            else 
            { 
                DisplayImage.gameObject.SetActive(false); 
            }
            
            SetAnswers();
        }
        else
        {
            QuizPanel.SetActive(false);
            GameOverPanel.SetActive(true);
            FinalScoreTxt.text = "Final Score\n\n\n\n You got " + score + " points!";
        }
    }
    #endregion
}