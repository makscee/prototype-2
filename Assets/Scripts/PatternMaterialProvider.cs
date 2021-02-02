using System;
using UnityEngine;

[ExecuteInEditMode]
public class PatternMaterialProvider : MonoBehaviour
{
    public float balance = 0.5f, frequency = 0.1f, speed = 0.01f;
    public Color tint = Color.white;
    
    public Material material;
    public SpriteRenderer sr;
    
    static readonly int Color0Property = Shader.PropertyToID("_Color0");
    static readonly int Color1Property = Shader.PropertyToID("_Color1");
    static readonly int BalanceProperty = Shader.PropertyToID("_Balance");
    static readonly int FrequencyProperty = Shader.PropertyToID("_Frequency");
    static readonly int TintProperty = Shader.PropertyToID("_Tint");
    static readonly int SpeedProperty = Shader.PropertyToID("_Speed");

    void OnEnable()
    {
        if (material == null)
        {
            material = new Material(GlobalConfig.Instance.shaderPatternMaterial);
            if (sr != null)
                sr.material = material;
        }

        SetShaderProperties();
    }

    void OnValidate()
    {
        if (material != null)
            SetShaderProperties();
    }

    public void SetShaderProperties()
    {
        material.SetColor(Color0Property, GlobalConfig.Instance.palette1);
        material.SetColor(Color1Property, GlobalConfig.Instance.palette3);
        material.SetFloat(BalanceProperty, balance);
        material.SetFloat(FrequencyProperty, frequency);
        material.SetColor(TintProperty, tint);
        material.SetFloat(SpeedProperty, speed);
    }
}