using UnityEngine;

public class ColorBlindnessController : MonoBehaviour
{
    public enum Mode { Normal, Protanopia, Deuteranopia, Tritanopia }

    [SerializeField] private Material material;
    [Range(0, 1f)] public float intensity = 1f;
    public Mode mode = Mode.Protanopia;

    static readonly int M0 = Shader.PropertyToID("_M0");
    static readonly int M1 = Shader.PropertyToID("_M1");
    static readonly int M2 = Shader.PropertyToID("_M2");
    static readonly int Intensity = Shader.PropertyToID("_Intensity");

    void Update()
    {
        if (material == null) return;

        material.SetFloat(Intensity, intensity);

        // ✅ default = Normal (prevents unassigned variable error)
        Vector4 r0 = new Vector4(1, 0, 0, 0);
        Vector4 r1 = new Vector4(0, 1, 0, 0);
        Vector4 r2 = new Vector4(0, 0, 1, 0);

        switch (mode)
        {
            case Mode.Protanopia:
                r0 = new Vector4(0.567f, 0.433f, 0f, 0f);
                r1 = new Vector4(0.558f, 0.442f, 0f, 0f);
                r2 = new Vector4(0f, 0.242f, 0.758f, 0f);
                break;

            case Mode.Deuteranopia:
                r0 = new Vector4(0.625f, 0.375f, 0f, 0f);
                r1 = new Vector4(0.700f, 0.300f, 0f, 0f);
                r2 = new Vector4(0f, 0.300f, 0.700f, 0f);
                break;

            case Mode.Tritanopia:
                r0 = new Vector4(0.950f, 0.050f, 0f, 0f);
                r1 = new Vector4(0f, 0.433f, 0.567f, 0f);
                r2 = new Vector4(0f, 0.475f, 0.525f, 0f);
                break;

            case Mode.Normal:
            default:
                // keep identity (already set)
                break;
        }

        material.SetVector(M0, r0);
        material.SetVector(M1, r1);
        material.SetVector(M2, r2);
    }
}