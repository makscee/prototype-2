using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShapeCellSidesContainer : MonoBehaviour
{
    [SerializeField] ShapeCellSide[] sides;
    [SerializeField] ShapeCellObject shapeCellObject;
    [SerializeField] GameObject darkenSprite;

    float Thickness => _thicknessOverride == -1
        ? GlobalConfig.Instance.thicknessBase +
          (shapeCellObject.shape.Field != null
              ? shapeCellObject.shape.Field.ShapeSidesThickness.value
              : 0f)
        : _thicknessOverride;
    float _thicknessOverride = -1f;
    bool _wrongSide;

    void Start()
    {
        RefreshSides();
        PostFxController.Instance.SubscribeToColors(RefreshColors);
    }

    void Update()
    {
        if (!_wrongSide) return;
        if (GameManager.flicker)
            ClearThicknessOverride();
        else SetThicknessOverride(0f);
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
        if (_thicknessOverride == value) return;
        _thicknessOverride = value;
        RefreshSides();
    }

    public void ClearThicknessOverride()
    {
        if (_thicknessOverride == -1f) return;
        _thicknessOverride = -1f;
        RefreshSides();
    }

    public void SetWrongSide(bool value)
    {
        _wrongSide = value;
        darkenSprite.SetActive(value);
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