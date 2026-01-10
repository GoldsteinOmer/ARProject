using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    [Header("Filter Settings")]
    [Tooltip("Drag the Colorblind Material here to reset it")]
    public Material filterMaterial;

    public void returnToMainMenu()
    {
        // 1. Reset the filter to Normal (0) before leaving the scene
        if (filterMaterial != null)
        {
            filterMaterial.SetInt("_Mode", 0);
            Debug.Log("Filter Mode Reset to Normal (0) before returning to menu.");
        }

        // 2. Load the Main Menu scene
        SceneManager.LoadSceneAsync(0);
    }
}