using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shape
{
    public Vector2Int originalPos;
    public int originalRotation;
    
    public Vector2Int pos;
    public ShapeObject shapeObject;
    Vector2Int upDirection = Vector2Int.up;
    public List<ShapeCell> cells = new List<ShapeCell>();
    public Vector2Int size;
    public Vector2Int ShapeRotationSize => new Vector2Int(Width, Height);

    public int Width => Mathf.RoundToInt((upDirection.Rotate90(true) * size).magnitude);
    public int Height => Mathf.RoundToInt((upDirection * size).magnitude);
    
    public Quaternion RotationQuaternion =>
        Quaternion.AngleAxis(RotationAngle, Vector3.forward);

    public float RotationAngle => -90f * Utils.DirFromCoords(UpDirection);
    
    public Shape(List<ShapeCell> cells)
    {
        this.cells = cells;
    }

    public FieldMatrix Field { get; set; }

    public Vector2Int UpDirection
    {
        get => upDirection;
        private set => upDirection = value;
    }

    public void AttachToMatrix()
    {
        shapeObject.SetParent(Field.transform);
        shapeObject.SetTargetPosition(pos + Field.ZeroPos);
        shapeObject.SetTargetScale(1f);
        shapeObject.transform.localRotation = Quaternion.identity;
    }

    public void SetRotation(Vector2Int dir)
    {
        while (upDirection != dir)
            RotateClockwise();
        foreach (var shapeCell in cells)
        {
            shapeCell.UpdateRotation();
        }
    }

    void RotateClockwise()
    {
        UpDirection = UpDirection.Rotate90(true);
        var deltaPos = -upDirection * (size.x - 1); 
        pos += deltaPos;
        shapeObject.DirectPositionOffset((Vector2)deltaPos);
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
        Field.UpdateShapePlacement(this);
    }

    public void PlaceShapeObject()
    {
        shapeObject.SetTargetPosition(Field.ZeroPos + pos);
    }

    public IEnumerable<Shape> Move(Vector2Int dir)
    {
        var toPush = new HashSet<Shape>();
        foreach (var shapeCell in cells)
        {
            if (shapeCell == null)
                continue;
            var occupiedBy = Field[shapeCell.FieldPos + dir]?.OccupiedBy;
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

    public bool CanMove(Vector2Int dir, int amount = 1, bool allowPush = true)
    {
        return Field == null || cells.All(cell => cell == null || cell.CanMove(dir, amount, allowPush));
    }

    public int MaxMoves(Vector2Int dir, bool allowPush = true)
    {
        var amount = 1;
        while (CanMove(dir, amount, allowPush)) amount++;

        return amount - 1;
    }

    public Vector2Int AddCell(Vector2Int localPos)
    {
        if (cells.Any(cell => cell.LocalPos == localPos)) return Vector2Int.zero;
        cells.Add(new ShapeCell(this, localPos));
        return RepackCells();
    }

    public Vector2Int RemoveCell(Vector2Int localPos)
    {
        var cell = cells.Find(c => c.LocalPos == localPos);
        if (!cells.Remove(cell)) return Vector2Int.zero;
        cell.Destroy();
        return RepackCells();
    }

    Vector2Int RepackCells()
    {
        var minPos = cells[0].LocalPos;
        var maxPos = minPos;
        foreach (var cell in cells)
        {
            minPos = new Vector2Int(Mathf.Min(minPos.x, cell.LocalPos.x), 
                Mathf.Min(minPos.y, cell.LocalPos.y));
            maxPos = new Vector2Int(Mathf.Max(maxPos.x, cell.LocalPos.x), 
                Mathf.Max(maxPos.y, cell.LocalPos.y));
        }

        size = maxPos - minPos + Vector2Int.one;
        var delta = -minPos;
        foreach (var cell in cells) cell.LocalPos += delta;
        return delta;
    }

    public void Destroy()
    {
        UnityEngine.Object.Destroy(shapeObject.gameObject);
        FieldMatrix.Active.RemoveShape(this);
    }
    
    public ShapeCell this[Vector2Int pos]
    {
        get => cells.FirstOrDefault(cell => cell.LocalPos == pos);
    }
}
