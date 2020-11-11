using System;
using UnityEngine;

public class FieldMatrix : MonoBehaviour
{
    public int sizeX, sizeY;
    public Shape attachedShape;
    public Vector2 ZeroPos => new Vector2(-(sizeX - 1) / 2f, -(sizeY - 1) / 2f);

    FieldCell[,] _cells;

    public void AttachShape(Shape shape)
    {
        shape.Matrix = this;
        attachedShape = shape;

        MoveAttachedShapeAccordingToDir(MaxShapeOffset / 2);
    }

    int MaxShapeOffset => attachedShape.UpDirection.x == 0 ? 
        sizeX - attachedShape.sizeX : sizeY - attachedShape.sizeX;
    int CurrentShapeOffset
    {
        get
        {
            if (attachedShape.UpDirection == Vector2Int.up)
                return attachedShape.X;
            if (attachedShape.UpDirection == Vector2Int.right)
                return sizeY - 1 - attachedShape.Y;
            if (attachedShape.UpDirection == Vector2Int.down)
                return sizeX - 1 - attachedShape.X;
            if (attachedShape.UpDirection == Vector2Int.left)
                return attachedShape.Y;
            return -1;
        }
    }

    void MoveAttachedShapeAccordingToDir(int offset)
    {
        var pos = Vector2Int.zero;
        if (attachedShape.UpDirection == Vector2Int.up)
            pos = new Vector2Int(offset, 0);
        else if (attachedShape.UpDirection == Vector2Int.right)
            pos = new Vector2Int(0, sizeY - 1 - offset);
        else if (attachedShape.UpDirection == Vector2Int.down)
            pos = new Vector2Int(sizeX - 1 - offset, sizeY - 1);
        else if (attachedShape.UpDirection == Vector2Int.left)
            pos = new Vector2Int(sizeX - 1, 0 + offset);

        var shapePos = pos - attachedShape.UpDirection * attachedShape.sizeY;
        attachedShape.Translate(shapePos.x, shapePos.y);
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
                attachedShape.UpDirection = attachedShape.UpDirection.Rotate90(!right);
                MoveAttachedShapeAccordingToDir(0);
            }
        }
        else
        {
            if (curOffset > 0)
                MoveAttachedShapeAccordingToDir(curOffset - 1);
            else
            {
                attachedShape.UpDirection = attachedShape.UpDirection.Rotate90(!right);
                MoveAttachedShapeAccordingToDir(MaxShapeOffset);
            }
        }
    }
    
    void OnValidate()
    {
        if (_cells == null || sizeX != _cells.GetLength(0) || sizeY != _cells.GetLength(1))
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
    }

    void CreateField()
    {
        if (this == null) return;
        foreach (var cell in GetComponentsInChildren<FieldCell>())
            cell.Destroy();
    
        _cells = new FieldCell[sizeX, sizeY];
        for (var x = 0; x < sizeX; x++)
        {
            for (var y = 0; y < sizeY; y++)
            {
                _cells[x, y] = FieldCell.Create(this, x, y);
            }
        }
    }

    public bool CheckIndex(int x, int y)
    {
        return x >= 0 && y >= 0 && x < _cells.GetLength(0) && y < _cells.GetLength(1);
    }

    public FieldCell this[int x, int y]
    {
        get => !CheckIndex(x, y) ? null : _cells[x, y];
    }
}