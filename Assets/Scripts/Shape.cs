using System.Linq;
using UnityEngine;

public class Shape
{
    public Vector2Int pos;
    FieldMatrix matrix;
    public ShapeObject shapeObject;
    Vector2Int upDirection = Vector2Int.up;
    public ShapeCell[,] cells;
    public Vector2Int size;
    public Color color;

    public int Width => Mathf.RoundToInt((upDirection.Rotate90(true) * size).magnitude);
    public int Height => Mathf.RoundToInt((upDirection * size).magnitude);

    Shape(string[] shapeCells)
    {
        color = Random.ColorHSV(0f, 1f, 0.5f, 1f);
        size = new Vector2Int(shapeCells[0].Length, shapeCells.Length);
        cells = new ShapeCell[size.x, size.y];
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
            shapeObject.transform.localRotation = Quaternion.identity;
        }
    }

    public Vector2Int UpDirection
    {
        get => upDirection;
        set
        {
            upDirection = value;
            // shapeObject.transform.localRotation = 
            //     Quaternion.AngleAxis(-90 * Utils.DirFromCoords(UpDirection), Vector3.forward);
        }
    }

    public bool InsertToMatrix()
    {
        var height = Height;
        var dir = upDirection;
        if (!CanMove(dir, height))
        {
            Debug.Log($"Cant move");
            return false;
        }
        for (var i = 0; i < height; i++)
        {
            Move(dir);
        }

        return true;
    }

    public void SetRotation(Vector2Int dir)
    {
        while (upDirection != dir)
            RotateClockwise();
    }

    public void RotateClockwise()
    {
        var newCells = new ShapeCell[size.y, size.x];
        UpDirection = UpDirection.Rotate90(true);
        pos -= upDirection * (size.x - 1);
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                var newX = y;
                var newY = size.x - x - 1;
                var cell = cells[x, y];
                if (cell == null) continue;
                cell.LocalPos = new Vector2Int(newX, newY);
                newCells[newX, newY] = cell;
            }
        }

        cells = newCells;
        size = new Vector2Int(size.y, size.x);
    }

    public void Translate(Vector2Int pos)
    {
        Translate(pos.x, pos.y);
    }
    public void Translate(int x, int y)
    {
        pos = new Vector2Int(x, y);
        shapeObject.transform.localPosition = new Vector2(x, y) + matrix.ZeroPos;
        // shapeObject.transform.localRotation = 
        //     Quaternion.AngleAxis(-90 * Utils.DirFromCoords(UpDirection), Vector3.forward);
        matrix.UpdateShapePlacement(this);
    }

    public void Move(Vector2Int dir)
    {
        foreach (var shapeCell in cells)
            if (shapeCell != null)
            {
                var occupiedBy = matrix[shapeCell.FieldPos + dir]?.OccupiedBy;
                if (occupiedBy != this) occupiedBy?.Move(dir);
            }
        Translate(pos + dir);
    }

    public bool CanMove(Vector2Int dir, int moves = 1)
    {
        return Matrix == null || cells.Cast<ShapeCell>().All(cell => cell == null || cell.CanMove(dir, moves));
    }

    public static Shape Create(string[] cells)
    {
        var shape = new Shape(cells);

        return shape;
    }
}

public static class ShapeStrings
{
    public static string[][] AllShapes = new string[][]
    {
        new []{
            "000",
            "0--"
        },
        new []{
            "000",
            "--0"
        },
        new []{
            "000",
            "-0-"
        },
        new []{
            "00",
            "00"
        },
    };
}