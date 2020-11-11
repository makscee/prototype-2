using System.Linq;
using UnityEngine;

public class Shape
{
    public int X, Y;
    public int sizeX, sizeY;
    FieldMatrix matrix;
    public bool onField;
    public ShapeObject shapeObject;
    Vector2Int upDirection = Vector2Int.up;
    ShapeCell[,] cells;

    Shape(string[] shapeCells)
    {
        sizeX = shapeCells[0].Length;
        sizeY = shapeCells.Length;
        cells = new ShapeCell[sizeX, sizeY];
        shapeObject = ShapeObject.Create(this);
        for (var i = 0; i < shapeCells.Length; i++)
        {
            var y = shapeCells.Length - i - 1;
            for (var x = 0; x < shapeCells[0].Length; x++)
            {
                if (shapeCells[i][x] == '-') continue;
                cells[x, y] = new ShapeCell(this, x, y);
            }
        }
    }

    public FieldMatrix Matrix
    {
        get => matrix;
        set
        {
            matrix = value;
            shapeObject.transform.SetParent(matrix.transform);
            Translate(X, Y);
        }
    }

    public Vector2Int UpDirection
    {
        get => upDirection;
        set
        {
            upDirection = value;
            shapeObject.transform.localRotation = 
                Quaternion.AngleAxis(-90 * Utils.DirFromCoords(UpDirection), Vector3.forward);
        }
    }

    public void Translate(int x, int y)
    {
        X = x;
        Y = y;
        shapeObject.transform.localPosition = new Vector2(x, y) + matrix.ZeroPos;
        shapeObject.transform.localRotation = 
            Quaternion.AngleAxis(-90 * Utils.DirFromCoords(UpDirection), Vector3.forward);
    }

    public bool CanMove(Vector2Int dir)
    {
        return Matrix == null || cells.Cast<ShapeCell>().All(cell => cell == null || cell.CanMove(dir));
    }

    public static Shape Create(string[] cells)
    {
        var shape = new Shape(cells);

        return shape;
    }
}

public static class ShapeStrings
{
    public static string[] L = {
        "000",
        "0--",
    };
    public static string[] J = {
        "000",
        "--0",
    };
}