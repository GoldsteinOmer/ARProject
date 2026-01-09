using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Main Screens")]
    public GameObject mainScreen;
    public GameObject quizScreen;
    public GameObject IntroScreen;
    public GameObject leaderboardScreen;
    public GameObject learnScreen;

    [Header("Learn Sub-Panels")]
    public GameObject normalVisionScreen;
    public GameObject deuteranopiaScreen;
    public GameObject protanopiaScreen;
    public GameObject tritanopiaScreen;

    [Header("Toggles")]
    public Toggle normalToggle; // Drag your 'Normal' Toggle here

    private List<GameObject> allMainScreens;
    private List<GameObject> allLearnSubScreens;

    void Start()
    {
        allMainScreens = new List<GameObject> { 
            mainScreen, quizScreen, IntroScreen, leaderboardScreen, learnScreen 
        };

        allLearnSubScreens = new List<GameObject> { 
            normalVisionScreen, deuteranopiaScreen, protanopiaScreen, tritanopiaScreen 
        };

        ShowScreen(mainScreen);
    }

    public void ShowScreen(GameObject screenToShow)
    {
        foreach (var screen in allMainScreens)
        {
            screen.SetActive(screen == screenToShow);
        }

        // NEW: If we are opening the Learn Screen, reset it to Normal
        if (screenToShow == learnScreen)
        {
            ResetLearnPanel();
        }
    }

    // This forces the UI back to Normal mode
    private void ResetLearnPanel()
    {
        // 1. Force the Toggle to 'On' (this will trigger ToggleNormal automatically)
        if (normalToggle != null)
        {
            normalToggle.isOn = true;
        }

        // 2. Ensure the correct sub-panel is showing
        OpenLearnVision(normalVisionScreen);
    }

    public void OpenLearnVision(GameObject subPanelToShow)
    {
        foreach (var subScreen in allLearnSubScreens)
        {
            subScreen.SetActive(subScreen == subPanelToShow);
        }
    }

    // --- TOGGLE HANDLERS ---
    public void ToggleNormal(bool isOn) { if (isOn) OpenLearnVision(normalVisionScreen); }
    public void ToggleDeuteranopia(bool isOn) { if (isOn) OpenLearnVision(deuteranopiaScreen); }
    public void ToggleProtanopia(bool isOn) { if (isOn) OpenLearnVision(protanopiaScreen); }
    public void ToggleTritanopia(bool isOn) { if (isOn) OpenLearnVision(tritanopiaScreen); }

    public void GoToMain() => ShowScreen(mainScreen);


    public void OpenLeaderboard()
    {
        // 1. Show the Leaderboard Screen (hides main menu)
        ShowScreen(leaderboardScreen);
        
        // 2. Find the QuizManager and tell it to display scores directly
        QuizManager2 quiz = FindFirstObjectByType<QuizManager2>(); 
        if (quiz != null)
        {
            quiz.OpenLeaderboardDirectly();
        }
    }
}