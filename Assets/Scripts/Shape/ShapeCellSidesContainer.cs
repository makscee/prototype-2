using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShapeCellSidesContainer : MonoBehaviour
{
    [SerializeField] ShapeCellSide[] sides;
    [SerializeField] ShapeCellObject shapeCellObject;

    float Thickness => _thicknessOverride == -1
        ? GlobalConfig.Instance.thicknessBase +
          (shapeCellObject.shape.Field != null
              ? shapeCellObject.shape.Field.ShapeSidesThickness.value
              : 0f)
        : _thicknessOverride;
    float _thicknessOverride = -1f;

    void Start()
    {
        RefreshSides();
        PostFxController.Instance.SubscribeToColors(RefreshColors);
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

    public void SetThicknessOverride(float value)
    {
        _thicknessOverride = value;
        RefreshSides();
    }

    public void ClearThicknessOverride()
    {
        _thicknessOverride = -1f;
        RefreshSides();
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
        if (PostFxController.Instance != null)
            PostFxController.Instance.UnsubscribeFromColors(RefreshColors);
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