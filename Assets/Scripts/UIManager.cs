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


    private List<GameObject> allScreens;

    void Start()
    {
        // Add all screens to a list to manage them easily
        allScreens = new List<GameObject> { mainScreen, normalVisionScreen, deuteranopiaScreen, protanopiaScreen, tritanopiaScreen, IntroScreen ,quizScreen };
        ShowScreen(mainScreen); // Start at main menu
    }

    public void ShowScreen(GameObject screenToShow)
    {
        foreach (var screen in allScreens)
        {
            screen.SetActive(screen == screenToShow);
        }
    }

    // Shortcut for Back Buttons
    public void GoToMain()
    {
        ShowScreen(mainScreen);
    }
}