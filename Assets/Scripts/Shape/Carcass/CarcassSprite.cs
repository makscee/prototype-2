using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class CarcassSprite : MonoBehaviour
{
    protected int sortingOrder;
    protected Color color;
    [SerializeField] SpriteRenderer sr;

    protected virtual void OnEnable()
    {
        Init();
    }

    void Init()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = sortingOrder;
        sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        sr.color = color;
    }
}