using System;
using UnityEngine;
using UnityEngine.U2D;

[ExecuteInEditMode]
public class CarcassSprite : MonoBehaviour
{
    protected int sortingOrder;
    protected Color color;
    [SerializeField] SpriteShapeRenderer sr;

    protected virtual void OnEnable()
    {
        Init();
    }

    void Init()
    {
        if (sr == null) sr = GetComponent<SpriteShapeRenderer>();
        sr.sortingOrder = sortingOrder;
        sr.color = color;
    }
}