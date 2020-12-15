using System;
using UnityEngine;

[ExecuteAlways]
public class CarcassSpriteContainer : MonoBehaviour
{
    [SerializeField] bool[] closedSides = new bool[4];

    public FrontCarcassSprite[] frontSprites;
    public BackCarcassSprite[] backSprites;

    void OnValidate()
    {
        Refresh();
    }

    void Refresh()
    {
        frontSprites = GetComponentsInChildren<FrontCarcassSprite>();
        backSprites = GetComponentsInChildren<BackCarcassSprite>();
    }
}