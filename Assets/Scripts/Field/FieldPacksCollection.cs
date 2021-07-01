using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class FieldPacksCollection : MonoBehaviour
{
    public static FieldPack[] Packs;
    // [SerializeField] bool initPacks;

    void Awake()
    {
        Packs = GetComponentsInChildren<FieldPack>();
        Packs[0].Activate();
    }

    void OnValidate()
    {
//         if (initPacks)
//         {
// #if UNITY_EDITOR
//             EditorApplication.delayCall += Init;
//             initPacks = false;
// #endif
//         }
    }

    public static FieldPack GetFirstIncompletePack()
    {
        for (var i = 0; i < Packs.Length; i++)
            if (!Packs[i].Complete)
                return Packs[i];
        return Packs.Last();
    }

    public static void DebugActivateFieldPack(int packId)
    {
        var pack = Packs[packId];
        pack.Activate();
        foreach (var field in pack.fields) field.SetCompletion(FieldCompletion.Unlocked);
        Progress.PauseTracking(true);
    }

    void Init()
    {
        foreach (var fieldPack in GetComponentsInChildren<FieldPack>())
            DestroyImmediate(fieldPack.gameObject);
        Packs = new FieldPack[2];
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
                fieldMatrix.SetScreenState(screenState);
            }
        }
    }
}