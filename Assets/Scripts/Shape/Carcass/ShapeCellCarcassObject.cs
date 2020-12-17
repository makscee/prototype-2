using System;
using UnityEngine;

[ExecuteInEditMode]
public class ShapeCellCarcassObject : MonoBehaviour
{
    [SerializeField] bool[] closedSides = new bool[4];
    [SerializeField] bool refresh;
    [SerializeField] CarcassOutlineSide[] sides;

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

    public void Refresh()
    {
        if (surroundingCells == null) return;
        
        for (var i = 0; i < 4; i++)
        {
            var dir1 = Utils.CoordsFromDirInt(i);
            var dir2 = dir1.Rotate90(false);
            sides[i].IsCorner =
                GetSurroundingCell(dir1) && GetSurroundingCell(dir2) && !GetSurroundingCell(dir1 + dir2);
            sides[i].Enable(sides[i].IsCorner || !GetSurroundingCell(dir1));
            sides[i].SetColor(GlobalConfig.Instance.palette3);
        }
    }
}