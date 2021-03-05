using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class PostFxController : MonoBehaviour
{
    [SerializeField] Volume volume;
    [SerializeField] int savePackId;
    [SerializeField] bool saveLiftGamma;
    [SerializeField] bool enableEdit;
    [SerializeField] int loadPackId;
    int _curPackId, _prevPackId = -1;
    float _t;
    public Color[] colors = new Color[2];

    static PostFxController _instanceCache;
    public static PostFxController Instance 
    {
        get
        {
            if (_instanceCache == null) _instanceCache = FindObjectOfType<PostFxController>();
            return _instanceCache;
        }
    }

    void Update()
    {
        if (enableEdit) return;
        if (_t < 1f)
            _t = Mathf.Min(_t + Time.deltaTime / GlobalConfig.Instance.paletteChangeTime, 1f);
        var palettes = FieldPackPalettes.Instance.palettes;
        PackPalette p;
        if (_prevPackId == -1)
            p = palettes[_curPackId];
        else
            p = PackPalette.Lerp(palettes[_prevPackId], palettes[_curPackId], _t);
        ApplyPalette(p);
    }

    void OnValidate()
    {
        if (saveLiftGamma)
        {
            saveLiftGamma = false;
            SavePalette();
        }

        if (_curPackId != loadPackId)
        {
            LoadPackPalette(loadPackId);
        }
        ApplyColors();
    }

    public void LoadPackPalette(int packId)
    {
        if (packId == _curPackId) return;
        _prevPackId = _curPackId;
        _curPackId = packId;
        loadPackId = packId;
        _t = 0;
    }

    Action<Color[]> _onColorChange;

    public void SubscribeToColors(Action<Color[]> action)
    {
        action.Invoke(colors);
        _onColorChange += action;
    }

    public void UnsubscribeFromColors(Action<Color[]> action)
    {
        _onColorChange -= action;
    }

    void ApplyColors()
    {
        _onColorChange?.Invoke(colors);
    }

    LiftGammaGain _lggCache;

    LiftGammaGain GetLiftGammaComponent()
    {
        if (_lggCache == null) volume.profile.TryGet(out _lggCache);
        return _lggCache;
    }

    Bloom _bloomCache;

    Bloom GetBloomComponent()
    {
        if (_bloomCache == null) volume.profile.TryGet(out _bloomCache);
        return _bloomCache;
    }

    Vignette _vignetteCache;

    Vignette GetVignetteComponent()
    {
        if (_vignetteCache == null) volume.profile.TryGet(out _vignetteCache);
        return _vignetteCache;
    }

    public void ApplyPalette(PackPalette v)
    {
        var lg = GetLiftGammaComponent();
        lg.lift.value = v.lift;
        lg.gamma.value = v.gamma;
        lg.gain.value = v.gain;

        var bloom = GetBloomComponent();
        bloom.intensity.value = v.bloomIntensity;
        bloom.threshold.value = v.bloomThreshold;
        bloom.scatter.value = v.bloomScatter;
        bloom.tint.value = v.bloomTint;

        var vignette = GetVignetteComponent();
        vignette.intensity.value = v.vignetteIntensity;
        vignette.smoothness.value = v.vignetteSmoothness;

        if (colors[0] != v.color0 || colors[1] != v.color1)
        {
            colors[0] = v.color0;
            colors[1] = v.color1;
            ApplyColors();
        }
    }

    void SavePalette()
    {
        var v = FieldPackPalettes.Instance.palettes[savePackId];
        var lg = GetLiftGammaComponent();
         v.lift = lg.lift.value;
        v.gamma = lg.gamma.value;
        v.gain = lg.gain.value;

        var bloom = GetBloomComponent();
        v.bloomIntensity = bloom.intensity.value;
        v.bloomThreshold = bloom.threshold.value;
        v.bloomScatter = bloom.scatter.value;
        v.bloomTint = bloom.tint.value;

        var vignette = GetVignetteComponent();
        v.vignetteIntensity = vignette.intensity.value;
        v.vignetteSmoothness = vignette.smoothness.value;

        v.color0 = colors[0];
        v.color1 = colors[1];

        FieldPackPalettes.Instance.palettes[savePackId] = v;
#if UNITY_EDITOR
        EditorUtility.SetDirty(FieldPackPalettes.Instance);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
}

[Serializable]
public struct PackPalette
{
    public Vector4 lift, gamma, gain;
    public Color color0, color1, bloomTint;
    public float vignetteIntensity, vignetteSmoothness, bloomThreshold, bloomIntensity, bloomScatter;

    public static PackPalette Lerp(PackPalette a, PackPalette b, float t)
    {
        if (t <= 0f) return a;
        if (t >= 1f) return b;
        var result = new PackPalette
        {
            lift = Vector4.Lerp(a.lift, b.lift, t),
            gamma = Vector4.Lerp(a.gamma, b.gamma, t),
            gain = Vector4.Lerp(a.gain, b.gain, t),
            
            color0 = Color.Lerp(a.color0, b.color0, t),
            color1 = Color.Lerp(a.color1, b.color1, t),
            bloomTint = Color.Lerp(a.bloomTint, b.bloomTint, t),
            
            vignetteIntensity = Mathf.Lerp(a.vignetteIntensity, b.vignetteIntensity, t),
            vignetteSmoothness = Mathf.Lerp(a.vignetteSmoothness, b.vignetteSmoothness, t),
            bloomThreshold = Mathf.Lerp(a.bloomThreshold, b.bloomThreshold, t),
            bloomIntensity = Mathf.Lerp(a.bloomIntensity, b.bloomIntensity, t),
            bloomScatter = Mathf.Lerp(a.bloomScatter, b.bloomScatter, t),
        };
        return result;
    }
}