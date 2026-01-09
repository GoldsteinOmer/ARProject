using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject normalVisionScreen;
    public GameObject deuteranopiaScreen;
    public GameObject protanopiaScreen;
    public GameObject tritanopiaScreen;
    public GameObject quizScreen;
    public GameObject IntroScreen;
    public GameObject leaderboardScreen;


    private List<GameObject> allScreens;

    void Start()
    {
        // Add all screens to a list to manage them easily
        allScreens = new List<GameObject> { mainScreen, normalVisionScreen, deuteranopiaScreen, protanopiaScreen, tritanopiaScreen, IntroScreen , quizScreen, leaderboardScreen };
        ShowScreen(mainScreen); // Start at main menu
    }

    public void OpenLeaderboard()
    {
        ShowScreen(leaderboardScreen);
        
        // Tell the QuizManager to display the scores without asking for a name
        QuizManager2 quiz = FindFirstObjectByType<QuizManager2>(); 
        if (quiz != null)
        {
            quiz.OpenLeaderboardDirectly();
        }
    }

    public void ShowScreen(GameObject screenToShow)
    {
        foreach (var screen in allScreens)
        {
            screen.SetActive(screen == screenToShow);
        }
    }

    public void GoToMain() => ShowScreen(mainScreen);
}