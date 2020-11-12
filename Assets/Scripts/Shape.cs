using System.Linq;
using UnityEngine;

public class Shape
{
    public Vector2Int pos;
    public int sizeX, sizeY;
    FieldMatrix matrix;
    public ShapeObject shapeObject;
    Vector2Int upDirection = Vector2Int.up;
    public ShapeCell[,] cells;

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

    public bool InsertToMatrix()
    {
        var height = sizeY;
        var dir = upDirection;
        RotateToUp();
        if (!CanMove(dir))
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

    void RotateToUp()
    {
        while (upDirection != Vector2Int.up)
            RotateClockwise();
    }

    public void RotateClockwise()
    {
        var newCells = new ShapeCell[sizeY, sizeX];
        UpDirection = UpDirection.Rotate90(false);
        pos -= upDirection * (sizeX - 1);
        Translate(pos);
        for (var x = 0; x < sizeX; x++)
        {
            for (var y = 0; y < sizeY; y++)
            {
                var newX = y;
                var newY = sizeX - x - 1;
                var cell = cells[x, y];
                if (cell == null) continue;
                cell.LocalPos = new Vector2Int(newX, newY);
                newCells[newX, newY] = cell;
            }
        }
        cells = newCells;
        sizeX = sizeY;
        sizeY = newCells.GetLength(1);
    }

    public void Translate(Vector2Int pos)
    {
        Translate(pos.x, pos.y);
    }
    public void Translate(int x, int y)
    {
        pos = new Vector2Int(x, y);
        shapeObject.transform.localPosition = new Vector2(x, y) + matrix.ZeroPos;
        shapeObject.transform.localRotation = 
            Quaternion.AngleAxis(-90 * Utils.DirFromCoords(UpDirection), Vector3.forward);
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