using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalConfig", menuName = "ScriptableObjects/GlobalConfig")]
public class GlobalConfig : ScriptableObject
{
    [SerializeField] public Color levelSelectorEntry1, levelSelectorEntry2;
    [SerializeField] public float shapeAnimationTime;
    [SerializeField] public float outlineThickness;
    [SerializeField] public Material shaderPatternMaterial;

    [SerializeField] public Color palette0, palette1, palette2, palette3;
    
    public static GlobalConfig Instance => GetInstance();

    static GlobalConfig _instanceCache;
    static GlobalConfig GetInstance()
    {
        if (_instanceCache == null)
            _instanceCache = Resources.Load<GlobalConfig>("GlobalConfig");
        return _instanceCache;
    }
}