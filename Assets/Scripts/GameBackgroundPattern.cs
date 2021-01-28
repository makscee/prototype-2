using System;
using UnityEngine;

[ExecuteInEditMode]
public class GameBackgroundPattern : MonoBehaviour
{
    public float balance = 0.5f, frequency = 0.1f, speed = 0.01f;
    public Color tint;
    
    Material _material;
    
    static readonly int Color0Property = Shader.PropertyToID("_Color0");
    static readonly int Color1Property = Shader.PropertyToID("_Color1");
    static readonly int BalanceProperty = Shader.PropertyToID("_Balance");
    static readonly int FrequencyProperty = Shader.PropertyToID("_Frequency");
    static readonly int TintProperty = Shader.PropertyToID("_Tint");
    static readonly int SpeedProperty = Shader.PropertyToID("_Speed");

    void OnEnable()
    {
        _material = new Material(GlobalConfig.Instance.shaderPatternMaterial);
        GetComponent<SpriteRenderer>().material = _material;
        SetShaderProperties();
    }

    void OnValidate()
    {
        if (_material != null)
            SetShaderProperties();
    }

    void SetShaderProperties()
    {
        _material.SetColor(Color0Property, GlobalConfig.Instance.palette1);
        _material.SetColor(Color1Property, GlobalConfig.Instance.palette3);
        _material.SetFloat(BalanceProperty, balance);
        _material.SetFloat(FrequencyProperty, frequency);
        _material.SetColor(TintProperty, tint);
        _material.SetFloat(SpeedProperty, speed);
    }
}