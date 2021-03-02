using System;
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
        SetLiftGamma(FieldPackPalettes.Instance.lift, FieldPackPalettes.Instance.gamma);
    }

    void OnValidate()
    {
        if (saveLiftGamma)
        {
            saveLiftGamma = false;
            SaveLiftGamma();
        }
    }

    void SaveLiftGamma()
    {
        var lg = GetLiftGammaComponent();
        FieldPackPalettes.Instance.lifts[savePackId] = lg.lift.value;
        FieldPackPalettes.Instance.gammas[savePackId] = lg.gamma.value;
    }

    LiftGammaGain _componentCache;
    LiftGammaGain GetLiftGammaComponent()
    {
        if (_componentCache == null) volume.profile.TryGet(out _componentCache);
        return _componentCache;
    }

    public void SetLiftGamma(Vector4 lift, Vector4 gamma)
    {
        var lg = GetLiftGammaComponent();
        lg.lift.value = lift;
        lg.gamma.value = gamma;
        lg.gain.value = new Vector4(1f, 1f, 1f, 0f);
        lg.gain.Release();
        // lg.lift.Override(lift);
        // lg.gamma.Override(gamma);
        // lg.gain.Override(new Vector4(1f, 1f, 1f, 0f));
    }
}