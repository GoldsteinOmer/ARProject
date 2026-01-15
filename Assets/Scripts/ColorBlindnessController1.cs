using UnityEngine;
using UnityEngine.UI; // <-- REQUIRED for UI elements

public class ColorBlindnessController1 : MonoBehaviour
{
    [Header("References")]
    public Image panelImage;      // <-- Drag your Panel/Image here
    public Material baseMaterial; // <-- Drag your ColorBlind Material here

    private Material instanceMaterial; // We create a copy so we don't mess up the original file
    private static readonly int MatrixPropID = Shader.PropertyToID("_ColorMatrix");

    // --- Matrices (Keep these exactly as you had them) ---
    private readonly Matrix4x4 normalVision = Matrix4x4.identity;
    private readonly Matrix4x4 protanopia = new Matrix4x4(
        new Vector4(0.152286f, 0.114503f, -0.003882f, 0.0f), 
        new Vector4(1.052583f, 0.786281f, -0.048116f, 0.0f), 
        new Vector4(-0.204868f, 0.099216f, 1.051998f, 0.0f), 
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );
    private readonly Matrix4x4 deuteranopia = new Matrix4x4(
        new Vector4(0.367322f, 0.280085f, -0.011820f, 0.0f), 
        new Vector4(0.860646f, 0.672501f, 0.042940f, 0.0f),  
        new Vector4(-0.227968f, 0.047413f, 0.968881f, 0.0f), 
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );
    private readonly Matrix4x4 tritanopia = new Matrix4x4(
        new Vector4(1.255528f, -0.078411f, 0.004733f, 0.0f), 
        new Vector4(-0.076749f, 0.930809f, 0.691367f, 0.0f), 
        new Vector4(-0.178779f, 0.147602f, 0.303900f, 0.0f), 
        new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
    );

    public enum ColorBlindMode { Normal, Protanopia, Deuteranopia, Tritanopia }

    [Header("Settings")]
    public ColorBlindMode currentMode = ColorBlindMode.Normal;

    void Awake()
    {
        // 1. Create a unique copy of the material so it only affects THIS panel
        if (baseMaterial != null)
        {
            instanceMaterial = new Material(baseMaterial);
            
            // 2. Assign that material to the Panel's Image component
            if (panelImage != null)
            {
                panelImage.material = instanceMaterial;
            }
        }
    }

    void Start() => SetMode(currentMode);

    public void SetMode(ColorBlindMode mode)
    {
        currentMode = mode;
        Matrix4x4 selectedMatrix = normalVision;

        switch (mode)
        {
            case ColorBlindMode.Protanopia: selectedMatrix = protanopia; break;
            case ColorBlindMode.Deuteranopia: selectedMatrix = deuteranopia; break;
            case ColorBlindMode.Tritanopia: selectedMatrix = tritanopia; break;
        }

        // Apply to our private instance, not the project asset
        if (instanceMaterial != null)
            instanceMaterial.SetMatrix(MatrixPropID, selectedMatrix);
    }

    private void OnDestroy()
    {
        // Clean up memory
        if (instanceMaterial != null) Destroy(instanceMaterial);
    }

    private void OnValidate()
    {
        if (Application.isPlaying) SetMode(currentMode);
    }

    // Button Functions
    public void SetNormal() => SetMode(ColorBlindMode.Normal);
    public void SetDeuteranopia() => SetMode(ColorBlindMode.Deuteranopia);
    public void SetProtanopia() => SetMode(ColorBlindMode.Protanopia);
    public void SetTritanopia() => SetMode(ColorBlindMode.Tritanopia);
}