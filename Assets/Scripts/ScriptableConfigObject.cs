using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ConfigObject", menuName = "ScriptableObjects/ConfigObject")]
public class ScriptableConfigObject : ScriptableObject
{
    [SerializeField] public Color levelSelectorEntry1, levelSelectorEntry2; 
    
    
    public static ScriptableConfigObject Instance => GetInstance();

    static ScriptableConfigObject _instanceCache;
    static ScriptableConfigObject GetInstance()
    {
        if (_instanceCache == null)
            _instanceCache = Resources.Load<ScriptableConfigObject>("ConfigObject");
        return _instanceCache;
    }
}