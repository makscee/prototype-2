using System;
using UnityEngine;

public class ShapeCellObject : MonoBehaviour
{
    public ShapeCell cell;
    public static ShapeCellObject Create(Vector2Int pos, Shape shape, ShapeCell cell)
    {
        var sco = Instantiate(Prefabs.Instance.shapeCellObject, shape.shapeObject.transform)
            .GetComponent<ShapeCellObject>();
        sco.transform.localPosition = (Vector2)pos;
        sco.cell = cell;
        return sco;
    }

    ShapeCellSidesContainer _sidesContainer;
    void Start()
    {
        _sidesContainer = GetComponentInChildren<ShapeCellSidesContainer>();
        InitInsides();
    }

    void InitInsides()
    {
        var surroundingCells = new bool[3, 3];
        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                surroundingCells[x + 1, y + 1] = cell.shape[cell.LocalPos + new Vector2Int(x, y)] != null;
            }
        }

        _sidesContainer.surroundingCells = surroundingCells;
        _sidesContainer.Refresh();
    }
}