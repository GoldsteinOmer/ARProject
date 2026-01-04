using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARSessionReset : MonoBehaviour
{
    private ARSession arSession;

    void Awake()
    {
        arSession = GetComponent<ARSession>();
    }

    void OnEnable()
    {
        if (arSession != null)
        {
            arSession.Reset();
        }
    }
}
