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
    public ShapeCell[,] cells;
    public Vector2Int size;
    public Color color;

    public int Width => Mathf.RoundToInt((upDirection.Rotate90(true) * size).magnitude);
    public int Height => Mathf.RoundToInt((upDirection * size).magnitude);

    Shape(string[] shapeCells)
    {
        color = Random.ColorHSV(0f, 1f, 0.3f, 0.5f, 1f, 1f);
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
            shapeObject.transform.localPosition = pos + matrix.ZeroPos;
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

        var toPush = new Dictionary<Shape, float>();
        toPush.Add(this, height);
        for (var i = 0; i < height; i++)
            foreach (var pushed in Move(dir))
            {
                if (!toPush.ContainsKey(pushed)) toPush.Add(pushed, 0);
                toPush[pushed] += 1f;
            }

        float pushAmount = height;
        Animator.Interpolate(0f, height, AnimationTime).PassDelta(v =>
        {
            pushAmount -= v;
            foreach (var shape in toPush.Keys.ToArray())
            {
                if (toPush[shape] > pushAmount)
                {
                    var delta = toPush[shape] - pushAmount;
                    shape.shapeObject.transform.localPosition += delta * (Vector3) (Vector2) dir;
                    toPush[shape] -= delta;
                }
            }
        }).Type(InterpolationType.InvSquare);

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
        var deltaPos = -upDirection * (size.x - 1); 
        pos += deltaPos;
        shapeObject.transform.localPosition += (Vector3)(Vector2)deltaPos;
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

    Interpolator<Vector2> _lastAnimation;

    void InvokeAfterLastAnimation(Action action)
    {
        if (_lastAnimation == null || _lastAnimation.IsDone())
            action();
        else _lastAnimation.whenDone += action;
    }

    void AnimateMoveShapeObject(Vector2Int deltaPos)
    {
        _lastAnimation = Animator.Interpolate(Vector2.zero, deltaPos, AnimationTime)
            .PassDelta(v => shapeObject.transform.localPosition += (Vector3) v)
            .Type(InterpolationType.Square)
            .ObjectStack(this);
    }
    
    public void Translate(Vector2Int newPos)
    {
        pos = newPos;
        matrix.UpdateShapePlacement(this);
    }

    public void PlaceShapeObject()
    {
        shapeObject.transform.localPosition = matrix.ZeroPos + pos;
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
        return Matrix == null || cells.Cast<ShapeCell>().All(cell => cell == null || cell.CanMove(dir, amount));
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
        new []{
            "00-",
            "-00"
        },
        new []{
            "-00",
            "00-"
        },
        new []{
            "0",
            "0",
            "0",
            "0"
        },
    };
}