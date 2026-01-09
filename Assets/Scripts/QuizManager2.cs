using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

[System.Serializable]
public class Question 
{
    public string fact;
    public bool isTrue;
}

public class QuizManager2 : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject quizPanel;
    public GameObject leaderboardPanel;

    [Header("Sub-Areas")]
    public GameObject nameEntryArea;
    public GameObject displayArea;

    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI progressScoreText;
    public TMP_InputField nameInputField; 
    public TextMeshProUGUI leaderboardListText;
    
    // NEW: Text to show the score and instructions inside NameEntryArea
    public TextMeshProUGUI endOfQuizMessage; 

    [Header("Feedback Settings")]
    public GameObject feedbackPanel;    
    public TextMeshProUGUI feedbackText; 
    public float delayDuration = 1.0f;
    public Color darkGreen = new Color(0f, 0.6f, 0f);

    [Header("Questions")]
    public List<Question> allQuestions; 
    
    private List<Question> shuffledQuestions;
    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;
    private bool isProcessing = false;

    private void Start()
    {
        if (feedbackPanel != null) feedbackPanel.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
    }

    public void StartQuiz()
    {
        correctAnswers = 0;
        currentQuestionIndex = 0;
        isProcessing = false;

        quizPanel.SetActive(true);
        leaderboardPanel.SetActive(false);
        
        if (feedbackPanel != null) feedbackPanel.SetActive(false);

        if (nameInputField != null) nameInputField.characterLimit = 10;

        if (allQuestions != null && allQuestions.Count > 0)
        {
            shuffledQuestions = allQuestions.OrderBy(x => Random.value).ToList();
            DisplayQuestion();
        }
    }

    void DisplayQuestion()
    {
        if (currentQuestionIndex < shuffledQuestions.Count)
        {
            questionText.text = shuffledQuestions[currentQuestionIndex].fact;
            progressScoreText.text = $"Score: {correctAnswers}/{currentQuestionIndex}";
        }
        else
        {
            // QUIZ FINISHED
            quizPanel.SetActive(false);
            leaderboardPanel.SetActive(true);
            nameEntryArea.SetActive(true);
            displayArea.SetActive(false);

            // UPDATED: Set the personalized message
            if (endOfQuizMessage != null)
            {
                endOfQuizMessage.text = $"You got {correctAnswers} out of {shuffledQuestions.Count} correct!\n\n" +
                                        "Enter your name to see if you made the Top 10!";
            }
        }
    }

    public void Answer(bool playerAnswer)
    {
        if (isProcessing || shuffledQuestions == null || currentQuestionIndex >= shuffledQuestions.Count) 
            return;

        bool isCorrect = (playerAnswer == shuffledQuestions[currentQuestionIndex].isTrue);
        if (isCorrect) correctAnswers++;

        StartCoroutine(HandleFeedback(isCorrect));
    }

    IEnumerator HandleFeedback(bool isCorrect)
    {
        isProcessing = true;
        if (feedbackPanel != null && feedbackText != null)
        {
            feedbackText.text = isCorrect ? "Correct!" : "Wrong!";
            feedbackText.color = isCorrect ? darkGreen : Color.red;
            feedbackPanel.SetActive(true);
        }

        yield return new WaitForSeconds(delayDuration);

        if (feedbackText != null) feedbackText.text = "";
        if (feedbackPanel != null) feedbackPanel.SetActive(false);

        currentQuestionIndex++;
        DisplayQuestion();
        isProcessing = false;
    }

    public void SubmitScore()
    {
        string playerName = nameInputField.text;
        if (string.IsNullOrWhiteSpace(playerName)) playerName = "Player";

        SaveScore(playerName, correctAnswers);
        
        nameEntryArea.SetActive(false);
        displayArea.SetActive(true);
        
        ShowLeaderboard();
    }

    void SaveScore(string name, int score)
    {
        string currentData = PlayerPrefs.GetString("LeaderboardData", "");
        currentData += $"{name}:{score},";
        PlayerPrefs.SetString("LeaderboardData", currentData);
        PlayerPrefs.Save();
    }

    public void ShowLeaderboard()
    {
        string rawData = PlayerPrefs.GetString("LeaderboardData", "");
        if (string.IsNullOrEmpty(rawData)) return;

        var scores = rawData.Split(',')
            .Where(s => s.Contains(":"))
            .Select(s => {
                var parts = s.Split(':');
                return new { Name = parts[0], Score = int.Parse(parts[1]) };
            })
            .OrderByDescending(x => x.Score)
            .Take(10)
            .ToList();

        leaderboardListText.text = "\n";
        
        for (int i = 0; i < scores.Count; i++)
        {
            leaderboardListText.text += $"{i + 1}. {scores[i].Name} - {scores[i].Score}\n";
        }
    }

    public void ResetQuizUI()
    {
        quizPanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        if (nameInputField != null) nameInputField.text = ""; 
    }

    public void OpenLeaderboardDirectly()
    {
        nameEntryArea.SetActive(false);
        displayArea.SetActive(true);
        ShowLeaderboard();
    }
}