using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    public List<QnA> QnA;
    public GameObject[] options;
    public int currentQuestion;

    public TextMeshProUGUI QuestionTxt;

    private void Start()
    {
        generateQuestion();
    }

    public void correct()
    {
        QnA.RemoveAt(currentQuestion);
        generateQuestion();
    }

    void SetAnswers()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<AnswersScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = QnA[currentQuestion].Answers[i];

            if(QnA[currentQuestion].CorrectAnswer == i+1)
            {
                options[i].GetComponent<AnswersScript>().isCorrect = true;
            }
        }
    }

    public RawImage DisplayImage; 

    void generateQuestion()
        {
        if (QnA.Count > 0)
        {
            currentQuestion = Random.Range(0, QnA.Count);
            QuestionTxt.text = QnA[currentQuestion].Question;
            
            // Update the image!
            if (QnA[currentQuestion].QuestionImage != null)
            {
                DisplayImage.texture = QnA[currentQuestion].QuestionImage;
                DisplayImage.gameObject.SetActive(true); // Show it if it exists
            }
            else
            {
                DisplayImage.gameObject.SetActive(false); // Hide it if no image is set
            }

            SetAnswers();
        }
        else
        {
            Debug.Log("Out of Questions!");
            QuestionTxt.text = "Quiz Complete!";
            
            // Hide the answer buttons so the player can't click them
            foreach (GameObject btn in options)
            {
                btn.SetActive(false);
            }
        }
    }
}
