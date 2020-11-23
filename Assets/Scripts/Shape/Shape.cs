using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shape
{
    public const float AnimationTime = 0.3f;
    
    public Vector2Int pos;
    FieldMatrix matrix;
    public ShapeObject shapeObject;
    Vector2Int upDirection = Vector2Int.up;
    public List<ShapeCell> cells = new List<ShapeCell>();
    public Vector2Int size;
    public Color color;

    public int Width => Mathf.RoundToInt((upDirection.Rotate90(true) * size).magnitude);
    public int Height => Mathf.RoundToInt((upDirection * size).magnitude);
    
    public Quaternion RotationQuaternion =>
        Quaternion.AngleAxis(RotationAngle, Vector3.forward);

    public float RotationAngle => -90f * Utils.DirFromCoords(UpDirection);

    Shape(string[] shapeCells)
    {
        CreateFromSerialized(ShapeSerialized.CreateFromString(shapeCells));
    }

    Shape(ShapeSerialized shapeSerialized)
    {
        CreateFromSerialized(shapeSerialized);
    }

    void CreateFromSerialized(ShapeSerialized shapeSerialized)
    {
        color = Random.ColorHSV(0f, 1f, 0.3f, 0.5f, 1f, 1f);
        size = new Vector2Int(shapeSerialized.sizeX, shapeSerialized.sizeY);
        shapeObject = ShapeObject.Create(this);
        foreach (var cellSerialized in shapeSerialized.cells)
            cells.Add(new ShapeCell(this, cellSerialized.x, cellSerialized.y));
    }

    public FieldMatrix Matrix
    {
        get => matrix;
        set
        {
            matrix = value;
            shapeObject.transform.SetParent(matrix.transform);
            shapeObject.targetPosition = pos + matrix.ZeroPos;
            shapeObject.transform.localRotation = Quaternion.identity;
        }
    }

    public Vector2Int UpDirection
    {
        get => upDirection;
        private set => upDirection = value;
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

        var moves = MaxMoves(dir);
        var toPush = new Dictionary<Shape, float>();
        toPush.Add(this, moves);
        for (var i = 0; i < moves; i++)
            foreach (var pushed in Move(dir))
            {
                if (!toPush.ContainsKey(pushed)) toPush.Add(pushed, 0);
                toPush[pushed] += 1f;
            }

        float pushAmount = moves;
        Animator.Interpolate(0f, moves, AnimationTime).PassDelta(v =>
        {
            pushAmount -= v;
            foreach (var shape in toPush.Keys.ToArray())
            {
                if (toPush[shape] > pushAmount)
                {
                    var delta = toPush[shape] - pushAmount;
                    shape.shapeObject.SetPositionDelta(delta * (Vector3) (Vector2) dir);
                    toPush[shape] -= delta;
                }
            }
        }).Type(InterpolationType.Square);

        return true;
    }

    public void SetRotation(Vector2Int dir)
    {
        while (upDirection != dir)
            RotateClockwise();
    }

    public void RotateClockwise()
    {
        UpDirection = UpDirection.Rotate90(true);
        var deltaPos = -upDirection * (size.x - 1); 
        pos += deltaPos;
        shapeObject.SetPositionDelta((Vector2)deltaPos);
        foreach (var cell in cells)
        {
            var newX = cell.LocalPos.y;
            var newY = size.x - cell.LocalPos.x - 1;
            cell.LocalPos = new Vector2Int(newX, newY);
        }
        size = new Vector2Int(size.y, size.x);
    }
    
    public void Translate(Vector2Int newPos)
    {
        pos = newPos;
        matrix.UpdateShapePlacement(this);
    }

    public void PlaceShapeObject()
    {
        shapeObject.targetPosition = matrix.ZeroPos + pos;
    }

    public IEnumerable<Shape> Move(Vector2Int dir)
    {
        var toPush = new HashSet<Shape>();
        foreach (var shapeCell in cells)
        {
            if (shapeCell == null)
                continue;
            var occupiedBy = matrix[shapeCell.FieldPos + dir]?.OccupiedBy;
            if (occupiedBy != null && occupiedBy != this)
                toPush.Add(occupiedBy);
        }
        var pushedByPush = new HashSet<Shape>();
        if (toPush.Count > 0)
            foreach (var moved in toPush.SelectMany(shape => shape.Move(dir)))
                pushedByPush.Add(moved);
        foreach (var shape in pushedByPush) toPush.Add(shape);
        Translate(pos + dir);
        return toPush;
    }

    public bool CanMove(Vector2Int dir, int amount = 1)
    {
        return Matrix == null || cells.All(cell => cell == null || cell.CanMove(dir, amount));
    }

    public int MaxMoves(Vector2Int dir)
    {
        var amount = 1;
        while (CanMove(dir, amount))
        {
            amount++;
        }

        return amount - 1;
    }

    public static Shape Create(string[] cells)
    {
        var shape = new Shape(cells);

        return shape;
    }
}

public static class ShapeStrings
{
    public static string[][] AllShapes =
    {
        new[]
        {
            "000",
            "0--"
        },
        new[]
        {
            "000",
            "--0"
        },
        new[]
        {
            "000",
            "-0-"
        },
        new[]
        {
            "00",
            "00"
        },
        new[]
        {
            "00-",
            "-00"
        },
        new[]
        {
            "-00",
            "00-"
        },
        new[]
        {
            "0",
            "0",
            "0",
            "0"
        },
    };
}