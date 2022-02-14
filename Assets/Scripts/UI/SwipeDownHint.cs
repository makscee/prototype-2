using System;
using UnityEngine;
using UnityEngine.UI;

public class SwipeDownHint : MonoBehaviour
{
    [SerializeField] RectTransform arrow;
    [SerializeField] RawImage arrowImage;
    [SerializeField] float animationTime = 2f;
    [SerializeField] float animationDelay = 0.5f;
    [SerializeField] float scaleStart = 0.5f;
    [SerializeField] float scaleFinish = 1f;
    [SerializeField] float offsetInScreenProportion = 0.3f;

    public bool IsPlaying { get; private set; }

    public void Play()
    {
        gameObject.SetActive(true);
        arrowImage.color = arrowImage.color.ChangeAlpha(0);
        IsPlaying = true;
        StartAnimation();
    }

    void Update()
    {
        if (IsPlaying && FieldMatrix.Active == null)
            Stop();
    }

    void Restart()
    {
        if (IsPlaying)
            StartAnimation();
    }

    void StartAnimation()
    {
        var height = Screen.height;
        Animator.Interpolate(0f, 1f, animationTime)
            .PassValue(v =>
            {
                arrow.anchoredPosition = new Vector2(0, -v * height * offsetInScreenProportion);
                var sc = scaleStart + (scaleFinish - scaleStart) * v;
                arrow.localScale = new Vector3(sc, sc, 1);
                if (v < 0.2)
                {
                    arrowImage.color = arrowImage.color.LerpAlpha(0f, 1f, v * 5);
                } else if (v < 0.7)
                {
                    arrowImage.color = arrowImage.color.ChangeAlpha(1);
                }
                else
                {
                    arrowImage.color = arrowImage.color.LerpAlpha(1f, 0f, (v - 0.7f) * 3.5f);
                }
            })
            .Delay(animationDelay)
            .Type(InterpolationType.Square)
            .WhenDone(Restart)
            .WhenStart(() => arrow.anchoredPosition = Vector2.zero);
    }

    public void Stop()
    {
        gameObject.SetActive(false);
        IsPlaying = false;
    }
}