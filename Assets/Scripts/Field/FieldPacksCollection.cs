using System;
using UnityEngine;

public class FieldPacksCollection : MonoBehaviour
{
    public static readonly FieldPack[] Packs = new FieldPack[5];

    void Awake()
    {
        Init();
    }

    void Init()
    {
        for (var i = 0; i < Packs.Length; i++)
        {
            var fp = FieldPack.Create(i);
            var scale = Mathf.Pow(4f, i);
            var t = fp.transform;
            t.SetParent(transform);
            t.localScale = new Vector3(scale, scale, scale);
            t.localPosition = Vector3.zero;
            Packs[i] = fp;
            fp.SetFieldsState();
        }

        FieldPack.active = Packs[0];
    }

    public static void PropagateFieldMatrixState(FieldState state, FieldMatrix except = null)
    {
        foreach (var fieldPack in Packs)
        {
            foreach (var fieldMatrix in fieldPack.fields)
            {
                if (fieldMatrix == except) continue;
                fieldMatrix.SetState(state);
            }
        }
    }
}