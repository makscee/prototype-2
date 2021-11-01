using System;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class PatternMaterialProvider : MonoBehaviour
{
    public float balance = 0f, frequency = 0.1f, speed = 0.01f;
    public float balanceTarget;
    public Color tint = Color.white;

    Material _material;
    public SpriteRenderer sr;
    public ParticleSystem ps;
    
    static readonly int Color0Property = Shader.PropertyToID("_Color0");
    static readonly int Color1Property = Shader.PropertyToID("_Color1");
    static readonly int BalanceProperty = Shader.PropertyToID("_Balance");
    static readonly int FrequencyProperty = Shader.PropertyToID("_Frequency");
    static readonly int TintProperty = Shader.PropertyToID("_Tint");
    static readonly int SpeedProperty = Shader.PropertyToID("_Speed");

    public Material Material
    {
        get
        {
            if (_material == null)
            {
                InitMaterial();
            }

            return _material;
        }
        private set => _material = value;
    }

    void InitMaterial()
    {
        _material = new Material(GlobalConfig.Instance.shaderPatternMaterial);
        if (sr != null)
            sr.material = _material;
        if (ps != null)
            ps.GetComponent<Renderer>().material = _material;
        balanceTarget = balance;
        SetShaderProperties();
        PostFxController.Instance.SubscribeToColors(SetColors);
    }

    void Start()
    {
        InitMaterial();
    }

    void OnValidate()
    {
        if (_material != null)
            SetShaderProperties();
    }

    void SetColors(IReadOnlyList<Color> colors)
    {
        Material.SetColor(Color0Property, colors[0]);
        Material.SetColor(Color1Property, colors[1]);
    }

    public void SetShaderProperties()
    {
        Material.SetFloat(BalanceProperty, balance);
        Material.SetFloat(FrequencyProperty, frequency);
        Material.SetColor(TintProperty, tint);
        Material.SetFloat(SpeedProperty, Application.isPlaying ? speed : 0f);
    }
}