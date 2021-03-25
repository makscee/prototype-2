using System;
using UnityEngine;

public class FieldUnlockedSprite : MonoBehaviour
{
    [SerializeField] PatternMaterialProvider pmp;
    [SerializeField] FieldMatrix field;

    void Awake()
    {
        field.onHoveredChange += HoverChangeHandle;
    }

    void HoverChangeHandle(bool value)
    {
        pmp.frequency = value ? 1 : 6;
        pmp.SetShaderProperties();
        pmp.transform.localScale = value ? new Vector3(1.2f, 1.2f) : Vector3.one;
    }
}