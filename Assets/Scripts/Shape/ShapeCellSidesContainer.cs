using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShapeCellSidesContainer : MonoBehaviour
{
    [SerializeField] bool refresh;
    [SerializeField] ShapeCellSide[] sides;
    [SerializeField] ShapeCellObject shapeCellObject;
    
    float Thickness => GlobalConfig.Instance.thicknessBase + shapeCellObject.shape.Field.ShapeSidesThickness.value;

    void Start()
    {
        RefreshSides();
        FieldPackPalettes.Instance.SubscribeToColors(RefreshColors);
    }

    void OnValidate()
    {
        refresh = false;
        Refresh();
    }

    public bool[,] surroundingCells;

    public bool GetSurroundingCell(Vector2Int pos)
    {
        return surroundingCells[1 + pos.x, 1 + pos.y];
    }

    public void RefreshSides()
    {
        if (shapeCellObject.shape == null) return;
        foreach (var side in sides)
        {
            side.Refresh(Thickness);
        }
    }

    void RefreshColors(IReadOnlyList<Color> colors)
    {
        for (var i = 0; i < 4; i++)
        {
            sides[i].SetColor(colors[0]);
        }
    }

    void OnDestroy()
    {
        FieldPackPalettes.Instance.UnsubscribeFromColors(RefreshColors);
    }

    public void Refresh()
    {
        if (surroundingCells == null) return;
        
        for (var i = 0; i < 4; i++)
        {
            var dir1 = Utils.CoordsFromDirInt(i);
            sides[i].Enable(GetSurroundingCell(dir1));
        }
    }
}