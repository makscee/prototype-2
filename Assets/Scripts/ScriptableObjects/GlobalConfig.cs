using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalConfig", menuName = "ScriptableObjects/GlobalConfig")]
public class GlobalConfig : ScriptableObject
{
    [SerializeField] public bool debug;
    [SerializeField] public Color levelSelectorEntry1, levelSelectorEntry2;

    [SerializeField] public float shapeAnimationTime,
        fieldCellsAnimationTime,
        sidesThicknessRecoverTime,
        balanceSetAnimationTime,
        fieldCompleteTransitionAnimationTime,
        fieldCompleteShapeThicknessBlinkTime;
    [SerializeField, Range(0f, 1f)] public float thickness, thicknessBase, sinScale, sinTimeScale;
    [SerializeField] public Material shaderPatternMaterial;
    
    [SerializeField] public float containerScale, containerOffsetX, containerOffsetY, containerPaddingY, containerSizeScale;

    [SerializeField] public float cameraShakeAmount, cameraFollowSpeed;
    [SerializeField] public float cameraFieldSizeMult, cameraFPSizeMult;
    [SerializeField] public float paletteChangeTime;
    
    public static GlobalConfig Instance => GetInstance();

    static GlobalConfig _instanceCache;
    static GlobalConfig GetInstance()
    {
        if (_instanceCache == null)
            _instanceCache = Resources.Load<GlobalConfig>("GlobalConfig");
        return _instanceCache;
    }

    public static Action onValidate;
    void OnValidate()
    {
        onValidate?.Invoke();
    }
}