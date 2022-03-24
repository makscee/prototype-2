using UnityEngine;

public class HelpCanvas : MonoBehaviour
{
    [SerializeField] ClipTexture[] clips;
    [SerializeField] GameObject controlsMobile, controlsPc;
    public bool isEnabled;

    public void Enable(bool value)
    {
        isEnabled = value;
        gameObject.SetActive(value);
        foreach (var clip in clips)
            if (value)
                clip.PlayClip();
            else clip.StopClip();
        var isMobile = Application.isMobilePlatform;
        controlsMobile.SetActive(isMobile);
        controlsPc.SetActive(!isMobile);
    }
}