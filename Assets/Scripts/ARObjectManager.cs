using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ARObjectManager : MonoBehaviour
{
    [Header("3D Model Settings")]
    public Transform earthModel;      
    public float rotationSpeed = 20f;

    [Header("Shader/Material Assets")]
    public Material filterMaterial;   
    private List<Material> instancedMaterials = new List<Material>();

    [Header("UI Toggle Settings")]
    public GameObject colorOptionsContainer; 
    public Toggle noneToggle, deutanToggle, protanToggle, tritanToggle;

    private static readonly int ModeID = Shader.PropertyToID("_Mode");

    void Start()
    {
        // 1. Setup Materials for all parts of the Earth
        if (earthModel != null && filterMaterial != null)
        {
            Renderer[] renderers = earthModel.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                Material newInst = new Material(filterMaterial);
                // Use _MainTex since your shader uses that property name
                if (rend.sharedMaterial.mainTexture != null)
                {
                    newInst.SetTexture("_MainTex", rend.sharedMaterial.mainTexture);
                }
                rend.material = newInst;
                instancedMaterials.Add(newInst);
            }
        }

        SetupToggles();
        SetShaderMode(0);
        if (noneToggle != null) noneToggle.isOn = true;
    }

    void SetupToggles()
    {
        if (noneToggle != null) noneToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetShaderMode(0); });
        if (deutanToggle != null) deutanToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetShaderMode(1); });
        if (protanToggle != null) protanToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetShaderMode(2); });
        if (tritanToggle != null) tritanToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetShaderMode(3); });
    }

    void Update()
    {
        if (earthModel != null)
        {
            // Spin around the Earth's OWN vertical axis
            // Space.Self is key here!
            earthModel.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    public void SetShaderMode(int modeValue)
    {
        foreach (Material mat in instancedMaterials)
        {
            if (mat != null) mat.SetInt(ModeID, modeValue);
        }
    }
}