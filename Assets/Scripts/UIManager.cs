using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Main Screens")]
    public GameObject mainScreen;
    public GameObject quizScreen;
    public GameObject IntroScreen;
    public GameObject leaderboardScreen;
    public GameObject learnScreen;

    [Header("Learn Sub-Panels (The Visuals)")]
    public GameObject normalVisionScreen;
    public GameObject deuteranopiaScreen;
    public GameObject protanopiaScreen;
    public GameObject tritanopiaScreen;

    [Header("Explanation Panels (The Text)")]
    public GameObject normalTextPanel;
    public GameObject deutanTextPanel;
    public GameObject protanTextPanel;
    public GameObject tritanTextPanel;

    [Header("Content Control")]
    public Material colorblindShaderMaterial; 
    public GameObject imageContainer;         
    public TextMeshProUGUI toggleButtonText;  

    [Header("Toggles")]
    public Toggle normalToggle;

    private List<GameObject> allMainScreens;
    private List<GameObject> allLearnSubScreens;
    private List<GameObject> allTextPanels;
    
    private GameObject currentActiveTextPanel;
    private bool showingText = false;

    void Start()
    {
        allMainScreens = new List<GameObject> { mainScreen, quizScreen, IntroScreen, leaderboardScreen, learnScreen };
        allLearnSubScreens = new List<GameObject> { normalVisionScreen, deuteranopiaScreen, protanopiaScreen, tritanopiaScreen };
        allTextPanels = new List<GameObject> { normalTextPanel, deutanTextPanel, protanTextPanel, tritanTextPanel };

        ShowScreen(mainScreen);
    }

    public void ShowScreen(GameObject screenToShow)
    {
        foreach (var screen in allMainScreens)
            screen.SetActive(screen == screenToShow);

        if (screenToShow == learnScreen)
            ResetLearnPanel();
    }

    private void ResetLearnPanel()
    {
        if (normalToggle != null) 
        {
            normalToggle.isOn = true;
        }
        
        showingText = false;
        
        // FIX: Pass BOTH arguments here to match the new function signature
        OpenLearnVision(normalVisionScreen, normalTextPanel);
        
        SetShaderMode(0); 
        UpdateContentVisibility();
    }

    public void SwitchContent()
    {
        showingText = !showingText;
        UpdateContentVisibility();
    }

    private void UpdateContentVisibility()
    {
        if(imageContainer != null) imageContainer.SetActive(!showingText);

        foreach (var panel in allTextPanels)
        {
            if(panel != null) panel.SetActive(false);
        }

        if (showingText && currentActiveTextPanel != null)
        {
            currentActiveTextPanel.SetActive(true);
        }

        if (toggleButtonText != null)
        {
            toggleButtonText.text = showingText ? "Show Image" : "Show Info";
        }
    }

    // This method now correctly requires two GameObjects
    public void OpenLearnVision(GameObject subPanelToShow, GameObject textPanelToMatch)
    {
        foreach (var subScreen in allLearnSubScreens)
        {
            if(subScreen != null) subScreen.SetActive(subScreen == subPanelToShow);
        }

        currentActiveTextPanel = textPanelToMatch;

        if (showingText)
        {
            UpdateContentVisibility();
        }
    }

    private void SetShaderMode(int mode)
    {
        if (colorblindShaderMaterial != null)
            colorblindShaderMaterial.SetInt("_Mode", mode);
    }

    // --- TOGGLE HANDLERS ---
    // These now correctly pass both the visual and the text objects
    public void ToggleNormal(bool isOn) { if (isOn) { OpenLearnVision(normalVisionScreen, normalTextPanel); SetShaderMode(0); } }
    public void ToggleDeuteranopia(bool isOn) { if (isOn) { OpenLearnVision(deuteranopiaScreen, deutanTextPanel); SetShaderMode(1); } }
    public void ToggleProtanopia(bool isOn) { if (isOn) { OpenLearnVision(protanopiaScreen, protanTextPanel); SetShaderMode(2); } }
    public void ToggleTritanopia(bool isOn) { if (isOn) { OpenLearnVision(tritanopiaScreen, tritanTextPanel); SetShaderMode(3); } }

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