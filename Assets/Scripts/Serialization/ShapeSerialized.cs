using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class ShapeSerialized : JsonUtilitySerializable
{
    public int sizeX, sizeY, originalX, originalY, originalRotation;
    public List<ShapeCellSerialized> cells = new List<ShapeCellSerialized>();

    public ShapeSerialized(int sizeX, int sizeY, int originalX = -1, int originalY = -1)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.originalX = originalX;
        this.originalY = originalY;
    }

    public ShapeSerialized(Shape shape)
    {
        var size = shape.ShapeRotationSize;
        sizeX = size.x;
        sizeY = size.y;
        originalX = shape.pos.x;
        originalY = shape.pos.y;
        originalRotation = Utils.DirFromCoords(shape.UpDirection);
        foreach (var shapeCell in shape.cells)
        {
            var pos = shapeCell.LocalPos
                .FromLocalFieldRotation(shape)
                .ToLocalShapeRotation();
            cells.Add(new ShapeCellSerialized(pos, shapeCell.LocalPos + shape.pos));
        }
        RepackCells(cells);
    }

    static void RepackCells(List<ShapeCellSerialized> cells)
    {
        var minPos = new Vector2Int(cells[0].x, cells[0].y);
        var maxPos = minPos;
        foreach (var cell in cells)
        {
            minPos = new Vector2Int(Mathf.Min(minPos.x, cell.x), 
                Mathf.Min(minPos.y, cell.y));
            maxPos = new Vector2Int(Mathf.Max(maxPos.x, cell.x), 
                Mathf.Max(maxPos.y, cell.y));
        }

        var size = maxPos - minPos + Vector2Int.one;
        var delta = -minPos;
        foreach (var cell in cells)
        {
            cell.x += delta.x;
            cell.y += delta.y;
        }
    }

    public Shape Deserialize()
    {
        var color = Random.ColorHSV(0f, 1f, 0.3f, 0.5f, 1f, 1f);
        var shapeCells = new List<ShapeCell>();
        var shape = new Shape(shapeCells) {
            color = color,
            size = new Vector2Int(sizeX, sizeY), 
            originalPos = new Vector2Int(originalX, originalY),
            originalRotation = originalRotation,
        };
        shape.shapeObject = ShapeObject.Create(shape);
        shapeCells.AddRange(cells.Select(cellSerialized => cellSerialized.Deserialize(shape)));
        return shape;
    }

    public static ShapeSerialized CreateFromString(string[] shapeCells)
    {
        var shapeSerialized = new ShapeSerialized(shapeCells[0].Length, shapeCells.Length);
        for (var i = 0; i < shapeCells.Length; i++)
        {
            var y = shapeCells.Length - i - 1;
            for (var x = 0; x < shapeCells[0].Length; x++)
            {
                if (shapeCells[i][x] == '-') continue;
                shapeSerialized.cells.Add(new ShapeCellSerialized(x, y));
            }
        }
        return shapeSerialized;
    }
}