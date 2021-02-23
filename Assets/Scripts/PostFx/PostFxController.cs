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

    static PostFxController _instanceCache;
    public static PostFxController Instance 
    {
        get
        {
            if (_instanceCache == null) _instanceCache = FindObjectOfType<PostFxController>();
            return _instanceCache;
        }
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

    LiftGammaGain GetLiftGammaComponent()
    {
        volume.profile.TryGet(out LiftGammaGain lg);
        return lg;
    }

    public void SetLiftGamma(Vector4 lift, Vector4 gamma)
    {
        var lg = GetLiftGammaComponent();
        lg.lift.Override(lift);
        lg.gamma.Override(gamma);
    }
}