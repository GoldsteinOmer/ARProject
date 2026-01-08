using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

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

    [Header("Questions")]
    public List<Question> allQuestions; 
    
    private List<Question> shuffledQuestions;
    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;

    public void StartQuiz()
    {
        correctAnswers = 0;
        currentQuestionIndex = 0;

        quizPanel.SetActive(true);
        leaderboardPanel.SetActive(false);

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
            
            // --- THE FIX ---
            // Shows: (Correct Answers) / (Questions Completed)
            // On the first question, it will show 0/0.
            progressScoreText.text = $"Score: {correctAnswers}/{currentQuestionIndex}";
        }
        else
        {
            quizPanel.SetActive(false);
            leaderboardPanel.SetActive(true);
            nameEntryArea.SetActive(true);
            displayArea.SetActive(false);
        }
    }

    public void Answer(bool playerAnswer)
    {
        if (shuffledQuestions == null || currentQuestionIndex >= shuffledQuestions.Count) return;
        
        // 1. Check if the answer is correct
        if (playerAnswer == shuffledQuestions[currentQuestionIndex].isTrue)
        {
            correctAnswers++;
        }
        
        // 2. Move to the next index
        currentQuestionIndex++;
        
        // 3. Update the text with the new score
        DisplayQuestion();
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
}