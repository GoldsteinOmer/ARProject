using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    public List<QnA> QnA;
    public GameObject[] options;
    public int currentQuestion;

    public GameObject StartPanel;
    public GameObject OptionsPanel;
    public GameObject QuizPanel;
    public GameObject GameOverPanel;

    public TextMeshProUGUI QuestionTxt;
    public TextMeshProUGUI ScoreTxt;
    public TextMeshProUGUI FinalScoreTxt;

    public RawImage DisplayImage;
    public Material colorBlindMat;

    private int score;
    private int questionsAnswered;

    private void Start()
    {
        colorBlindMat.SetFloat("_Mode", 0);
        StartPanel.SetActive(true);
        OptionsPanel.SetActive(false);
        QuizPanel.SetActive(false);
        GameOverPanel.SetActive(false);
    }

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
        generateQuestion();
    }

    public void MainMenu() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }

    // Use THESE specific functions for your 4 Toggles
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

    public void correct()
    {
        score++;
        questionsAnswered++;
        UpdateScoreText();
        QnA.RemoveAt(currentQuestion);
        generateQuestion();
    }

    public void wrong()
    {
        questionsAnswered++;
        UpdateScoreText();
        QnA.RemoveAt(currentQuestion);
        generateQuestion();
    }

    void UpdateScoreText() { ScoreTxt.text = "Score: " + score + " / " + questionsAnswered; }

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
            else { DisplayImage.gameObject.SetActive(false); }
            SetAnswers();
        }
        else
        {
            QuizPanel.SetActive(false);
            GameOverPanel.SetActive(true);
            FinalScoreTxt.text = "Final Score: " + score + " / " + questionsAnswered;
        }
    }
}