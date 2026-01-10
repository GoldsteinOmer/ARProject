using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class TrackedImageController : MonoBehaviour
{
    private ARTrackedImageManager arTrackedImageManager; 

    [Header("Placement Adjustments")]
    public Vector3 rotationCorrection = new Vector3(90, 0, 0); // Fixes the -90 degree tilt
    public float heightAboveImage = 0.05f; // Moves object 5cm above the physical image

    void Awake()
    {
        arTrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        if (arTrackedImageManager != null)
        {
            #pragma warning disable 0618
            arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
            #pragma warning restore 0618
        }
    }

    void OnDisable()
    {
        if (arTrackedImageManager != null)
        {
            #pragma warning disable 0618
            arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            #pragma warning restore 0618
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // 1. Fix the Tilt and Height
            // We adjust the child object so the trackedImage (the parent) stays aligned with AR tracking
            foreach (Transform child in trackedImage.transform)
            {
                // Move the child 'up' relative to the image surface
                child.localPosition = new Vector3(0, heightAboveImage, 0);
                
                // Rotate the child to stand upright
                child.localEulerAngles = rotationCorrection;
            }

            // 2. Existing UI Logic
            SpawnUIImage uiScript = trackedImage.GetComponentInChildren<SpawnUIImage>();
            if (uiScript != null)
            {
                Vector2 physicalSize = trackedImage.size;
                uiScript.trackedImagePhysicalSize = physicalSize;
                
                float halfWidthInMeters = physicalSize.x / 2.0f;
                float uiScale = uiScript.uiScaleFactor * 10f; 
                uiScript.desiredButtonStartX = halfWidthInMeters * uiScale;
            }
        }
        
        // Use 'updated' as well in case the image is lost and regained
        foreach (var trackedImage in eventArgs.updated)
        {
             if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
             {
                 // Ensure children stay corrected during updates if they get reset
                 foreach (Transform child in trackedImage.transform)
                 {
                     child.localPosition = new Vector3(0, heightAboveImage, 0);
                     child.localEulerAngles = rotationCorrection;
                 }
             }
        }
    }
}