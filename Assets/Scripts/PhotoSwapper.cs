using UnityEngine;
using UnityEngine.UI;

public class PhotoSwapper : MonoBehaviour
{
    [Header("Target UI Element")]
    public RawImage displayPanel; // Drag RawImage here

    [Header("Photo Library")]
    public Texture2D photo1;
    public Texture2D photo2;
    public Texture2D photo3;
    public Texture2D photo4;

    public void ShowPhoto1() { displayPanel.texture = photo1; }
    public void ShowPhoto2() { displayPanel.texture = photo2; }
    public void ShowPhoto3() { displayPanel.texture = photo3; }
    public void ShowPhoto4() { displayPanel.texture = photo4; }
}
