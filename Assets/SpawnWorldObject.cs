using UnityEngine;
using UnityEngine.UI;

public class SpawnUIImage : MonoBehaviour
{
    // --- UI/Asset References ---
    public Button spawnButton;              // The original toggle button (Right Side)
    public RectTransform spawnParent;        // The Canvas RectTransform where the image will be spawned

    // --- Shader/Material Assets ---
    [Header("Shader Filter Assets")]
    [Tooltip("The material using the Custom/ColorblindObject shader.")]
    public Material filterMaterial;         // <-- NEW: Reference to the Material
    public Texture2D originalTexture;       // The main unfiltered texture
    
    // REMOVED: deutanTexture, protanTexture, tritanTexture
    
    // --- Toggle Group & Controls ---
    [Header("Color Filter Toggles")]
    [Tooltip("The parent GameObject containing the ToggleGroup and Toggles.")]
    public GameObject colorOptionsContainer; 
    public Toggle noneToggle;
    public Toggle deutanToggle;
    public Toggle protanToggle;
    public Toggle tritanToggle;
    
    // --- Image Size Settings ---
    [Header("Image Size Settings")]
    // ... (rest of the size settings remain the same)
    public Vector2 defaultImageSize = new Vector2(531, 334); 
    public float uiScaleFactor = 100f; 

    [HideInInspector]
    public Vector2 trackedImagePhysicalSize; 

    // --- Button Position Settings ---
    // ... (rest of the position settings remain the same)
    public float buttonVerticalOffset = 0f; 
    public float rightButtonHorizontalOffset = 50f;
    public float toggleGroupOffset = 50f; 
    
    [HideInInspector]
    public float desiredButtonStartX = 0f; // Right edge position of the tracked image

    private GameObject spawnedImage; 
    private RectTransform buttonRectTransform; 
    private RectTransform toggleContainerRectTransform;
    private Image spawnedImageComponent; 
    private Sprite originalSprite; 
    private Material currentFilterMaterialInstance; // <-- NEW: Instance to control shader
    
    // --- Shader Property IDs ---
    private static readonly int ModeID = Shader.PropertyToID("_Mode"); // Cache the shader property ID
    
    // --- Shader Mode Values ---
    // These correspond to the values in your shader: 0=None, 1=Deuteranopia, 2=Protanopia, 3=Tritanopia
    private const int MODE_NONE = 0;
    private const int MODE_DEUTAN = 1; 
    private const int MODE_PROTAN = 2; 
    private const int MODE_TRITAN = 3; 

    void Start()
    {
        // 1. Get RectTransforms and set pivots (Unchanged)
        buttonRectTransform = spawnButton.GetComponent<RectTransform>();
        buttonRectTransform.pivot = new Vector2(0f, buttonRectTransform.pivot.y); 
        
        toggleContainerRectTransform = colorOptionsContainer.GetComponent<RectTransform>();
        toggleContainerRectTransform.pivot = new Vector2(0f, toggleContainerRectTransform.pivot.y);

        // 2. Calculate and apply SPAWN BUTTON position (Unchanged)
        float spawnButtonLeftX = desiredButtonStartX + rightButtonHorizontalOffset;
        buttonRectTransform.localPosition = new Vector2(spawnButtonLeftX, buttonVerticalOffset); 
        
        // 3. Set the Toggle Group to its initial position (Unchanged)
        float spawnButtonWidth = buttonRectTransform.sizeDelta.x * buttonRectTransform.localScale.x;
        float toggleGroupLeftX = spawnButtonLeftX + spawnButtonWidth + toggleGroupOffset;
        toggleContainerRectTransform.localPosition = new Vector2(toggleGroupLeftX, buttonVerticalOffset);

        // 4. Set up click listeners (Unchanged)
        spawnButton.onClick.AddListener(ToggleImage);

        // 5. Set up Toggle listeners to control the SHADER MODE
        noneToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetShaderMode(MODE_NONE); });
        deutanToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetShaderMode(MODE_DEUTAN); }); // Set Mode 1
        protanToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetShaderMode(MODE_PROTAN); }); // Set Mode 2
        tritanToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetShaderMode(MODE_TRITAN); }); // Set Mode 3
        
        // **NOTE**: I've swapped Protan and Deutan values (1 and 2) to match the order in your C# list for better readability, 
        // but you should check your shader and make sure the values match the filter you want.
        // Your shader has: 1=Protanopia, 2=Deuteranopia, 3=Tritanopia. The code above respects this.
        
        // 6. Initial Visibility Setup (Unchanged)
        colorOptionsContainer.SetActive(false);
    }
    
    /// <summary>
    /// Sets the _Mode property on the spawned image's material.
    /// </summary>
    void SetShaderMode(int modeValue)
    {
        if (currentFilterMaterialInstance != null)
        {
            // Set the shader property value
            currentFilterMaterialInstance.SetInt(ModeID, modeValue);
        }
        else
        {
            Debug.LogWarning($"Cannot set shader mode {modeValue}. No image spawned or material instance is missing.");
        }
    }

    // REMOVED: SetFilterImage - The functionality is now in SetShaderMode

    /// <summary>
    /// Toggles the main spawned image AND the filter options visibility.
    /// </summary>
    void ToggleImage()
    {
        // If the image already exists → delete it
        if (spawnedImage != null)
        {
            // Clean up the dynamically created material instance
            if (currentFilterMaterialInstance != null)
            {
                Destroy(currentFilterMaterialInstance); // Destroy the instance, not the asset
                currentFilterMaterialInstance = null;
            }
            
            Destroy(spawnedImage);
            spawnedImage = null;
            spawnedImageComponent = null; 
            noneToggle.isOn = true;
            
            // Hide the filter options when the image is despawned
            colorOptionsContainer.SetActive(false);
            return;
        }

        // Otherwise → create it
        SpawnImage();
        
        // Show the filter options when the image is spawned
        colorOptionsContainer.SetActive(true);
    }

    void SpawnImage()
    {
        // 1. Create the Sprite from the original texture (Unchanged)
        originalSprite = Sprite.Create(
            originalTexture,
            new Rect(0, 0, originalTexture.width, originalTexture.height),
            new Vector2(0.5f, 0.5f)
        );

        // 2. Create the image GameObject (Unchanged)
        spawnedImage = new GameObject("SpawnedImage", typeof(Image));
        spawnedImageComponent = spawnedImage.GetComponent<Image>();
        spawnedImageComponent.sprite = originalSprite;
        
        // **NEW STEP**: Create a material instance and assign the shader
        if (filterMaterial != null)
        {
            // Create an instance of the material so we don't change the original asset
            currentFilterMaterialInstance = new Material(filterMaterial);
            spawnedImageComponent.material = currentFilterMaterialInstance;
        }
        else
        {
            Debug.LogError("Filter Material not assigned in the Inspector! Shader filtering will not work.");
        }
        
        // 3. Set the parent (the Canvas) (Unchanged)
        spawnedImage.transform.SetParent(spawnParent, false);

        RectTransform rt = spawnedImage.GetComponent<RectTransform>();
        
        // 4. Determine the size (Unchanged)
        Vector2 finalSize;
        if (trackedImagePhysicalSize.magnitude > 0.001f) 
        {
            finalSize = trackedImagePhysicalSize * uiScaleFactor * 10f; 
        }
        else
        {
            finalSize = defaultImageSize;
            Debug.LogWarning("Tracked Image Physical Size not set. Using default size.");
        }

        // 5. Apply the size and reset scale/rotation (Unchanged)
        rt.sizeDelta = finalSize;
        spawnedImage.transform.localScale = Vector3.one;
        spawnedImage.transform.localRotation = Quaternion.identity;
        
        // Ensure the "None" toggle is selected on spawn, which now calls SetShaderMode(MODE_NONE).
        noneToggle.isOn = true;
    }
}