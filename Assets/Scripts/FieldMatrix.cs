using System;
using UnityEngine;

public class FieldMatrix : MonoBehaviour
{
    public static FieldMatrix current;
    
    public Vector2Int size;
    public Shape attachedShape;
    public Vector2 ZeroPos => new Vector2(-(size.x - 1) / 2f, -(size.y - 1) / 2f);

    FieldCell[,] _cells;

    public void AttachShape(Shape shape, int offset, Vector2Int dir)
    {
        shape.Matrix = this;
        attachedShape = shape;
        shape.SetRotation(dir);
        MoveAttachedShapeAccordingToDir(offset);
    }

    int MaxShapeOffset => Mathf.RoundToInt((attachedShape.UpDirection.Rotate90(true) * size).magnitude -
                          (attachedShape.UpDirection.Rotate90(true) * attachedShape.size).magnitude);
    int CurrentShapeOffset =>
        Mathf.RoundToInt(Vector2Int.Distance(attachedShape.pos, ZeroOffsetPos(attachedShape.UpDirection)));

    Vector2Int ZeroOffsetPos(Vector2Int upDir)
    {
        var zeroOffsetPos = (-(upDir + upDir.Rotate90(true)) + Vector2Int.one) / 2 * (size - Vector2Int.one) - upDir;
        var startOffset = upDir.Rotate90(true) - upDir;
        startOffset.Clamp(-Vector2Int.one, Vector2Int.zero);
        zeroOffsetPos +=  startOffset * (attachedShape.size - Vector2Int.one);
        return zeroOffsetPos;
    }
    void MoveAttachedShapeAccordingToDir(int offset)
    {
        var shapePos = ZeroOffsetPos(attachedShape.UpDirection) + attachedShape.UpDirection.Rotate90(true) * offset;
        attachedShape.Translate(shapePos);
        attachedShape.PlaceShapeObject();
    }

    public void InsertShape()
    {
        if (attachedShape == null) throw new Exception("No shape attached");
        var curOffset = CurrentShapeOffset;
        if (attachedShape.InsertToMatrix())
        {
            AttachShape(Shape.Create(ShapeStrings.AllShapes.Random()), curOffset, attachedShape.UpDirection);
        }
    }

    public void MoveAttachedShape(bool right)
    {
        if (attachedShape == null) throw new Exception("No shape attached");
        var curOffset = CurrentShapeOffset;
        if (right)
        {
            if (curOffset < MaxShapeOffset)
                MoveAttachedShapeAccordingToDir(curOffset + 1);
            else
            {
                attachedShape.SetRotation(attachedShape.UpDirection.Rotate90(false));
                MoveAttachedShapeAccordingToDir(0);
            }
        }
        else
        {
            if (curOffset > 0)
                MoveAttachedShapeAccordingToDir(curOffset - 1);
            else
            {
                attachedShape.SetRotation(attachedShape.UpDirection.Rotate90(true));
                MoveAttachedShapeAccordingToDir(MaxShapeOffset);
            }
        }
    }

    public void UpdateShapePlacement(Shape shape)
    {
        foreach (var cell in _cells)
            if (cell.OccupiedBy == shape)
                cell.OccupiedBy = null;
        foreach (var shapeCell in shape.cells)
            if (shapeCell != null && CheckIndex(shapeCell.FieldPos))
                this[shapeCell.FieldPos].OccupiedBy = shape;
    }
    
    void OnValidate()
    {
        if (_cells == null || size.x != _cells.GetLength(0) || size.y != _cells.GetLength(1))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += CreateField;
#else 
            CreateField();
#endif
        }
    }

    void OnEnable()
    {
        CreateField();
        current = this;
        var shape = Shape.Create(ShapeStrings.AllShapes.Random());
        AttachShape(shape, shape.Width / 2, Vector2Int.up);
    }

    void OnDisable()
    {
        if (current == this) current = null;
    }

    void CreateField()
    {
        if (this == null) return;
        foreach (var cell in GetComponentsInChildren<FieldCell>())
            cell.Destroy();
    
        _cells = new FieldCell[size.x, size.y];
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                _cells[x, y] = FieldCell.Create(this, x, y);
            }
        }
    }

    public bool CheckIndex(int x, int y)
    {
        return x >= 0 && y >= 0 && x < _cells.GetLength(0) && y < _cells.GetLength(1);
    }

    public bool CheckIndex(Vector2Int pos)
    {
        return CheckIndex(pos.x, pos.y);
    }

    public FieldCell this[int x, int y]
    {
        get => !CheckIndex(x, y) ? null : _cells[x, y];
    }
    public FieldCell this[Vector2Int pos]
    {
        get => this[pos.x, pos.y];
    }
}