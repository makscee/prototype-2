using System;
using UnityEngine;

public class FieldMatrix : MonoBehaviour
{
    public static FieldMatrix current;
    
    public Vector2Int size;
    public ShapeContainer shapesContainer;
    public Shape attachedShape;
    public Material patternMaterial;
    public Vector2 ZeroPos => new Vector2(-(size.x - 1) / 2f, -(size.y - 1) / 2f);

    FieldCell[,] _cells;

    public Vector2Int MatrixAttachLocalPosition =>
        ZeroOffsetPos + currentShapeDir.Rotate90(true) * currentShapeOffset;


    int MaxShapeOffset => Mathf.RoundToInt((attachedShape.UpDirection.Rotate90(true) * size).magnitude -
                                           (attachedShape.UpDirection.Rotate90(true) * attachedShape.size).magnitude);

    public int currentShapeOffset;

    public Vector2Int currentShapeDir = Vector2Int.up;

    Vector2Int AttachedShapePosition => MatrixAttachLocalPosition + ShapeStartOffset(attachedShape);

    Vector2Int ZeroOffsetPos =>
        (-(currentShapeDir + currentShapeDir.Rotate90(true)) + Vector2Int.one) / 2 * (size - Vector2Int.one) -
        currentShapeDir;

    Vector2Int ShapeStartOffset(Shape shape)
    {
        var upDir = shape.UpDirection;
        var startOffset = upDir.Rotate90(true) - upDir;
        startOffset.Clamp(-Vector2Int.one, Vector2Int.zero);
        return startOffset * (attachedShape.size - Vector2Int.one);
    }

    public void AttachShape(Shape shape)
    {
        shape.Matrix = this;
        shape.AttachToMatrix();
        attachedShape = shape;
        shape.SetRotation(currentShapeDir);
        shape.shapeObject.SetTargetScale(Vector3.one);
        MoveAttachedShapeAccordingToDir(currentShapeOffset);
    }

    public void MoveAttachedShapeAccordingToDir(int offset)
    {
        currentShapeOffset = Mathf.Min(offset, MaxShapeOffset);
        currentShapeDir = attachedShape.UpDirection;
        attachedShape.Translate(AttachedShapePosition);
        attachedShape.PlaceShapeObject();
        RefreshProjection();
    }


    MoveTracker _moveTracker = new MoveTracker();
    public void InsertShape()
    {
        if (attachedShape == null) return;
        var move = new ShapeMove(this, attachedShape).Do(); 
        if (move != null)
        {
            var shape = shapesContainer.GetNext();
            if (shape != null)
                AttachShape(shape);
            else attachedShape = null;
            _moveTracker.AddMove(move);
        }
    }

    public void Undo()
    {
        if (_moveTracker.Moves == 0) return;
        if (attachedShape != null)
            shapesContainer.ReturnPrevious();
        var direction = _moveTracker.Last.direction;
        currentShapeOffset = _moveTracker.Last.offset;
        currentShapeDir = direction;
        var shape = _moveTracker.Undo();
        AttachShape(shape);
    }

    public void MoveAttachedShape(bool right)
    {
        if (attachedShape == null) return;
        var curOffset = currentShapeOffset;
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

        RefreshProjection();
    }

    public void RefreshProjection()
    {
        if (attachedShape == null) return;
        var maxMoves = attachedShape.MaxMoves(currentShapeDir, false);
        var point1 = ZeroOffsetPos + currentShapeDir.Rotate90(true) * currentShapeOffset + attachedShape.UpDirection;
        var point2 = point1 + (maxMoves - 1) * currentShapeDir +
                     currentShapeDir.Rotate90(true) * (attachedShape.Width - 1);

        var xMin = Mathf.Min(point1.x, point2.x);
        var xMax = Mathf.Max(point1.x, point2.x);
        var yMin = Mathf.Min(point1.y, point2.y);
        var yMax = Mathf.Max(point1.y, point2.y);

        foreach (var cell in _cells)
        {
            var val = 0;
            if (cell.X >= xMin && cell.X <= xMax && cell.Y >= yMin && cell.Y <= yMax)
                val = 1;
            cell.SetProjection(val);
        }

        var maxMoveVec = maxMoves * currentShapeDir;
        foreach (var shapeCell in attachedShape.cells)
        {
            var pos = shapeCell.LocalPos + attachedShape.pos + maxMoveVec;
            if (CheckIndex(pos))
                this[pos].SetProjection(2);
        }
    }

    public void UpdateShapePlacement(Shape shape)
    {
        RemoveShape(shape);
        foreach (var shapeCell in shape.cells)
            if (shapeCell != null && CheckIndex(shapeCell.FieldPos))
                this[shapeCell.FieldPos].OccupiedBy = shape;
    }

    public void RemoveShape(Shape shape)
    {
        foreach (var cell in _cells)
            if (cell.OccupiedBy == shape)
                cell.OccupiedBy = null;
    }
    
    void OnValidate()
    {
        if (_cells == null || size.x != _cells.GetLength(0) || size.y != _cells.GetLength(1))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += CreateField;
#endif
        }
    }

    void OnEnable()
    {
        CreateField();
        current = this;

        var container = ShapeContainerSerialized.GetLastLoadedLevel()?.Deserialize(this);
        
        if (container != null)
            SetContainer(container);
    }

    public void SetContainer(ShapeContainer container)
    {
        shapesContainer?.Destroy();
        shapesContainer = container;
        size = container.matrixSize;
        CreateField();
        
        var shape = shapesContainer.GetNext();
        currentShapeOffset = shape.Width / 2; 
        AttachShape(shape);
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
        RefreshProjection();
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