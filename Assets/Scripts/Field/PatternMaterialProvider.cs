using System;
using UnityEngine;

public class PatternMaterialProvider : MonoBehaviour
{
    public Material material;
    float _balance = .5f;
    
    static readonly int Color0Property = Shader.PropertyToID("_Color0");
    static readonly int Color1Property = Shader.PropertyToID("_Color1");
    static readonly int BalanceProperty = Shader.PropertyToID("_Balance");

    float Balance
    {
        get => _balance;
        set
        {
            _balance = value;
            material.SetFloat(BalanceProperty, value);
        }
    }

    void Awake()
    {
        material = new Material(GlobalConfig.Instance.shaderPatternMaterial);
        material.SetColor(Color0Property, GlobalConfig.Instance.palette1);
        material.SetColor(Color1Property, GlobalConfig.Instance.palette3);
        material.SetFloat(BalanceProperty, Balance);
    }

    public Interpolator<float> SetBalanceAnimated(float value)
    {
        Animator.ClearByOwner(this);
        var interpolator = Animator.Interpolate(Balance, value, GlobalConfig.Instance.balanceSetAnimationTime)
            .PassDelta(v => Balance += v)
            .Type(InterpolationType.InvSquare);
        interpolator.SetOwner(this);
        return interpolator;
    }
}