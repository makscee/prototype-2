using System;
using UnityEngine;

[ExecuteInEditMode]
public class ShapeCellSidesContainer : MonoBehaviour
{
    [SerializeField] bool refresh;
    [SerializeField] ShapeCellSide[] sides;
    [SerializeField] ShapeCellObject shapeCellObject;
    
    float Thickness => GlobalConfig.Instance.thicknessBase + shapeCellObject.shape.Field.shapeSidesThickness;

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

    void Update()
    {
        RefreshSides();
    }

    public void RefreshSides()
    {
        foreach (var side in sides)
        {
            side.Refresh(Thickness);
        }
    }

    public void Refresh()
    {
        if (surroundingCells == null) return;
        
        for (var i = 0; i < 4; i++)
        {
            var dir1 = Utils.CoordsFromDirInt(i);
            sides[i].Enable(GetSurroundingCell(dir1));
            sides[i].SetColor(GlobalConfig.Instance.palette1);
        }
    }
}