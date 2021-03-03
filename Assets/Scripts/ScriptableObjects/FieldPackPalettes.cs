using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FieldPackPalettes", menuName = "ScriptableObjects/FieldPackPalettes")]
public class FieldPackPalettes : ScriptableObject
{
    public PackPalette[] palettes = new PackPalette[5]; 
    public static FieldPackPalettes Instance => GetInstance();
    static FieldPackPalettes _instanceCache;
    static FieldPackPalettes GetInstance()
    {
        if (_instanceCache == null)
            _instanceCache = Resources.Load<FieldPackPalettes>("FieldPackPalettes");
        return _instanceCache;
    }
}