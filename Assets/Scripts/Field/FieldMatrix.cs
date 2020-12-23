using System;
using UnityEditor;
using UnityEngine;

public class FieldMatrix : MonoBehaviour
{
    static FieldMatrix _active;
    
    public int size;
    public ShapeContainer shapesContainer;
    public Shape attachedShape;
    public Material patternMaterial;
    public Vector2 ZeroPos => new Vector2(-(size - 1) / 2f, -(size - 1) / 2f);

    FieldCell[,] _cells;

    public Vector2Int MatrixAttachLocalPosition =>
        ZeroOffsetPos + currentShapeDir.Rotate90(true) * currentShapeOffset;


    int MaxShapeOffset => Mathf.RoundToInt((attachedShape.UpDirection.Rotate90(true) * size).magnitude -
                                           (attachedShape.UpDirection.Rotate90(true) * attachedShape.size).magnitude);

    public int currentShapeOffset;

    public Vector2Int currentShapeDir = Vector2Int.up;

    Vector2Int AttachedShapePosition => MatrixAttachLocalPosition + ShapeStartOffset(attachedShape);

    Vector2Int ZeroOffsetPos =>
        (-(currentShapeDir + currentShapeDir.Rotate90(true)) + Vector2Int.one) / 2 * new Vector2Int(size - 1, size - 1) -
        currentShapeDir;

    public static Action onActiveFieldSet;
    public static FieldMatrix Active
    {
        get => _active;
        set
        {
            _active = value;
            onActiveFieldSet?.Invoke();
        }
    }

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
        if (attachedShape == null)
        {
            SetCellsState(FieldCellState.ActiveEmpty);
            return;
        }
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
            var val = FieldCellState.ActiveEmpty;
            if (cell.X >= xMin && cell.X <= xMax && cell.Y >= yMin && cell.Y <= yMax)
                val = FieldCellState.ShapeProjectionTrail;
            cell.SetState(val);
        }

        var maxMoveVec = maxMoves * currentShapeDir;
        foreach (var shapeCell in attachedShape.cells)
        {
            var pos = shapeCell.LocalPos + attachedShape.pos + maxMoveVec;
            if (CheckIndex(pos))
                this[pos].SetState(FieldCellState.ShapeProjection);
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
        if (_cells == null || size != _cells.GetLength(0) || size != _cells.GetLength(1))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += CreateCells;
#endif
        }
    }

    void OnEnable()
    {
        onActiveFieldSet += NewActiveFieldHandle;
    }

    void OnDisable()
    {
        if (Active == this) Active = null;
        onActiveFieldSet -= NewActiveFieldHandle;
    }

    void NewActiveFieldHandle()
    {
        if (_active != this) SetState(FieldState.OnSelectScreen);
    }

    public void SetContainer(ShapeContainer container)
    {
        shapesContainer?.Destroy();
        shapesContainer = container;
        size = container.matrixSize;
        CreateCells();
        
        var shape = shapesContainer.GetNext();
        if (shape != null)
        {
            currentShapeOffset = shape.Width / 2;
            AttachShape(shape);
        }
    }

    Transform _cellParent;
    void CreateCells()
    {
        if (this == null ||
            PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.NotAPrefab &&
            PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.NotAPrefab) return;
        
        if (_cellParent == null)
        {
            _cellParent = new GameObject("Cells Container").transform;
            _cellParent.SetParent(transform);
            _cellParent.localPosition = Vector3.zero; 
            _cellParent.localRotation = Quaternion.identity;
        }
        foreach (var cell in _cellParent.GetComponentsInChildren<FieldCell>())
            cell.Destroy();
    
        _cells = new FieldCell[size, size];
        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                _cells[x, y] = FieldCell.Create(this, x, y, _cellParent);
            }
        }
        RefreshProjection();
    }

    public void SetState(FieldState state)
    {
        switch (state)
        {
            case FieldState.Active:
                gameObject.SetActive(true);
                shapesContainer.SetEnabled(true);
                attachedShape?.shapeObject.gameObject.SetActive(true);
                RefreshProjection();
                Active = this;
                break;
            case FieldState.Disabled:
                gameObject.SetActive(false);
                break;
            case FieldState.OnSelectScreen:
                gameObject.SetActive(true);
                shapesContainer.SetEnabled(false);
                attachedShape?.shapeObject.gameObject.SetActive(false);
                SetCellsState(FieldCellState.SelectScreen);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    void SetCellsState(FieldCellState state)
    {
        foreach (var cell in _cells) cell.SetState(state);
    }

    public void Destroy()
    {
        DestroyImmediate(gameObject);
    }

    public static FieldMatrix Create()
    {
        var field = Instantiate(Prefabs.Instance.fieldMatrix).GetComponent<FieldMatrix>();
        return field;
    }

    bool CheckIndex(int x, int y)
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