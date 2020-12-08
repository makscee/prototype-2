using System;
using UnityEngine;

[Serializable]
public class ShapeCellSerialized : JsonUtilitySerializable
{
    public int x, y, originalX, originalY;

    public ShapeCellSerialized(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public ShapeCellSerialized(Vector2Int pos, Vector2Int originalPos)
    {
        x = pos.x;
        y = pos.y;
        originalX = originalPos.x;
        originalY = originalPos.y;
    }

    public ShapeCellSerialized(ShapeCell shapeCell)
    {
        x = shapeCell.LocalPos.x;
        y = shapeCell.LocalPos.y;
        
    }

    public ShapeCell Deserialize(Shape shape)
    {
        return new ShapeCell(shape, new Vector2Int(x, y)) { originalPos = new Vector2Int(originalX, originalY)};
    }
}