using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class FieldPacksCollection : MonoBehaviour
{
    public static FieldPack[] Packs;
    [SerializeField] bool initPacks;

    void OnEnable()
    {
        Packs = GetComponentsInChildren<FieldPack>();
        FieldPack.active = Packs[0];
    }

    void OnValidate()
    {
        if (initPacks)
        {
#if UNITY_EDITOR
            EditorApplication.delayCall += Init;
            initPacks = false;
#endif
        }
    }

    void Init()
    {
        foreach (var fieldPack in GetComponentsInChildren<FieldPack>())
            DestroyImmediate(fieldPack.gameObject);
        Packs = new FieldPack[5];
        for (var i = 0; i < Packs.Length; i++)
        {
            var fp = FieldPack.Create(i);
            var scale = Mathf.Pow(4f, i);
            var t = fp.transform;
            t.SetParent(transform);
            t.localScale = new Vector3(scale, scale, scale);
            t.localPosition = Vector3.zero;
        }
    }

    public static void PropagateFieldMatrixState(FieldScreenState screenState, FieldMatrix except = null)
    {
        foreach (var fieldPack in Packs)
        {
            foreach (var fieldMatrix in fieldPack.fields)
            {
                if (fieldMatrix == except) continue;
                fieldMatrix.SetState(screenState);
            }
        }
    }
}