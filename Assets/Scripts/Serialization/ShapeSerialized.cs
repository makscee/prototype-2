using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class ShapeSerialized : JsonUtilitySerializable
{
    public int sizeX, sizeY;
    public List<ShapeCellSerialized> cells = new List<ShapeCellSerialized>();

    public ShapeSerialized(int sizeX, int sizeY)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
    }

    public ShapeSerialized(Shape shape)
    {
        sizeX = shape.size.x;
        sizeY = shape.size.y;
        foreach (var shapeCell in shape.cells) cells.Add(new ShapeCellSerialized(shapeCell));
    }

    public Shape Deserialize()
    {
        var color = Random.ColorHSV(0f, 1f, 0.3f, 0.5f, 1f, 1f);
        var shapeCells = new List<ShapeCell>();
        var shape = new Shape(shapeCells) {color = color, size = new Vector2Int(sizeX, sizeY)};
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