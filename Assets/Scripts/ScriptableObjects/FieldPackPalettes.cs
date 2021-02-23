using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FieldPackPalettes", menuName = "ScriptableObjects/FieldPackPalettes")]
public class FieldPackPalettes : ScriptableObject
{
    public Color[] colors0, colors1;
    public Vector4[] lifts, gammas;
    
    public static FieldPackPalettes Instance => GetInstance();
    static FieldPackPalettes _instanceCache;
    static FieldPackPalettes GetInstance()
    {
        if (_instanceCache == null)
            _instanceCache = Resources.Load<FieldPackPalettes>("FieldPackPalettes");
        return _instanceCache;
    }

    Action<Color[]> _onColorChange;
    [SerializeField] int currentPackId;
    int _prevPackId = -1;
    public Color[] Colors => new[] {colors0[currentPackId], colors1[currentPackId]};
    Color[] PrevColors => new[] {colors0[_prevPackId], colors1[_prevPackId]};

    public void LoadPackPalette(int packId)
    {
        currentPackId = packId;
    }

    public void SubscribeToColors(Action<Color[]> action)
    {
        action.Invoke(Colors);
        _onColorChange += action;
    }

    public void UnsubscribeFromColors(Action<Color[]> action)
    {
        _onColorChange -= action;
    }

    void Apply()
    {
        if (_prevPackId == currentPackId) return;
        if (_prevPackId == -1 || !Application.isPlaying)
        {
            _onColorChange?.Invoke(Colors);
            PostFxController.Instance.SetLiftGamma(lifts[currentPackId], gammas[currentPackId]);
        }
        else
        {
            var prev = _prevPackId;
            var cur = currentPackId;
            var prevC = PrevColors;
            var curC = Colors;
            Animator.ClearByOwner(this);
            Animator.Interpolate(0f, 1f, GlobalConfig.Instance.paletteChangeTime)
                .PassValue(v =>
                {
                    var lift = Vector4.Lerp(lifts[prev], lifts[cur], v);
                    var gamma = Vector4.Lerp(gammas[prev], gammas[cur], v);
                    PostFxController.Instance.SetLiftGamma(lift, gamma);
                    var colors = new[]
                        {Color.Lerp(prevC[0], curC[0], v), Color.Lerp(prevC[1], curC[1], v)};
                    _onColorChange?.Invoke(colors);
                }).SetOwner(this);
        }

        _prevPackId = currentPackId;
    }

    void OnEnable()
    {
        Apply();
    }

    void OnValidate()
    {
        Apply();
    }
}