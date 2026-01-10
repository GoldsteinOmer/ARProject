using UnityEngine;
using UnityEngine.SceneManagement;

public class ARSceneSwitcher : MonoBehaviour
{
    public string targetSceneName;
    
    [Header("Reset Settings")]
    public Material filterMaterial; // Drag your Colorblind material here

    public void SwitchScene()
    {
        // 1. Force the shader back to Normal (Mode 0) before leaving
        if (filterMaterial != null)
        {
            filterMaterial.SetInt("_Mode", 0);
            Debug.Log("Shader reset to Normal.");
        }

        // 2. Also try to find the Manager in the current scene to reset it properly
        ARObjectManager manager = Object.FindFirstObjectByType<ARObjectManager>();
        if (manager != null)
        {
            manager.SetShaderMode(0);
        }

        // 3. Load the new scene
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}