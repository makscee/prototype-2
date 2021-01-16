using System;
using UnityEngine;

public class FieldPacksLeveler : MonoBehaviour
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
        }

        FieldPack.active = Packs[0];
    }
}