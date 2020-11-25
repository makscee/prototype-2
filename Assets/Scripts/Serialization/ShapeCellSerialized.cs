using System;
using UnityEngine;

[Serializable]
public class ShapeCellSerialized : JsonUtilitySerializable
{
    public int x, y;

    public ShapeCellSerialized(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public ShapeCellSerialized(ShapeCell shapeCell)
    {
        x = shapeCell.LocalPos.x;
        y = shapeCell.LocalPos.y;
    }

    public ShapeCell Deserialize(Shape shape)
    {
        return new ShapeCell(shape, new Vector2Int(x, y));
    }
}