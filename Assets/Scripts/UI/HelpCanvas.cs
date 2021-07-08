using UnityEngine;

public class HelpCanvas : MonoBehaviour
{
    [SerializeField] ClipTexture[] clips;
    public bool isEnabled;

    public void Enable(bool value)
    {
        isEnabled = value;
        gameObject.SetActive(value);
        foreach (var clip in clips)
            if (value)
                clip.PlayClip();
            else clip.StopClip();
    }
}