using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Filter Settings")]
    [Tooltip("Drag the Colorblind Material here to reset it on scene change")]
    public Material filterMaterial;

    private void ResetFilter()
    {
        // This checks if the material is assigned. 
        // If it is, it sets the shader's _Mode back to 0 (Normal).
        if (filterMaterial != null)
        {
            filterMaterial.SetInt("_Mode", 0);
            Debug.Log("Filter Mode Reset to Normal (0)");
        }
    }

    public void ScanWorld()
    {
        ResetFilter();
        SceneManager.LoadSceneAsync(1);
    }

    public void takeATest()
    {
        ResetFilter();
        SceneManager.LoadSceneAsync(2);
    }

    public void knowledge()
    {
        ResetFilter();
        SceneManager.LoadSceneAsync(3);
    }

    public void Full()
    {
        ResetFilter();
        SceneManager.LoadSceneAsync(4);
    }
}