using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

// Attach this script to the GameObject that has the ARTrackedImageManager component
public class TrackedImageController : MonoBehaviour
{
    private ARTrackedImageManager arTrackedImageManager; 

    void Awake()
    {
        arTrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        if (arTrackedImageManager != null)
        {
            // Subscribing to the older, but currently functional, event.
            #pragma warning disable 0618
            arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
            #pragma warning restore 0618
        }
    }

    void OnDisable()
    {
        if (arTrackedImageManager != null)
        {
            // Unsubscribing from the older event.
            #pragma warning disable 0618
            arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            #pragma warning restore 0618
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Logic runs for every newly detected image
        foreach (var trackedImage in eventArgs.added)
        {
            SpawnUIImage uiScript = trackedImage.GetComponentInChildren<SpawnUIImage>();

            if (uiScript != null)
            {
                Vector2 physicalSize = trackedImage.size;
                uiScript.trackedImagePhysicalSize = physicalSize;
                
                float halfWidthInMeters = physicalSize.x / 2.0f;
                
                // CRUCIAL: Get the scale factor from the UI script and multiply by 10
                float uiScale = uiScript.uiScaleFactor * 10f; 

                // Calculate RIGHT EDGE position (for the spawn button)
                uiScript.desiredButtonStartX = halfWidthInMeters * uiScale;

                // NOTE: desiredButtonEndX calculation is no longer needed/calculated.
            }
        }
    }
}