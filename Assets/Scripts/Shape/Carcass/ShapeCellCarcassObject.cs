using System;
using UnityEngine;

[ExecuteInEditMode]
public class ShapeCellCarcassObject : MonoBehaviour
{
    public bool[] closedSides = new bool[4];
    [SerializeField] bool refresh;
    [SerializeField] SpriteRenderer outline;
    [SerializeField] OutlineMaskObject outlineMask;

    void OnValidate()
    {
        refresh = false;
        Refresh();
    }

    void Refresh()
    {
        outline.color = GlobalConfig.Instance.palette3;
        outlineMask.SetClosedSides(closedSides);
    }
}