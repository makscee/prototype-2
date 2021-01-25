using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class FieldMatrix : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject backgroundInputSprite, completionSprite;
    public int packId, fieldId;
    public ShapeContainer shapesContainer;
    public Shape attachedShape;

    public FieldScreenState screenState;    
    public FieldCompletion completion;

    public FieldMatrix[] dependencies;
    public Vector2 ZeroPos => new Vector2(-(Size - 1) / 2f, -(Size - 1) / 2f);

    FieldCell[,] _cells;

    public Vector2Int MatrixAttachLocalPosition =>
        ZeroOffsetPos + currentShapeDir.Rotate90(true) * currentShapeOffset;


    int MaxShapeOffset => Mathf.RoundToInt((attachedShape.UpDirection.Rotate90(true) * Size).magnitude -
                                           (attachedShape.UpDirection.Rotate90(true) * attachedShape.size).magnitude);


    public int currentShapeOffset;

    public Vector2Int currentShapeDir = Vector2Int.up;

    Vector2Int AttachedShapePosition => MatrixAttachLocalPosition + ShapeStartOffset(attachedShape);

    Vector2Int ZeroOffsetPos =>
        (-(currentShapeDir + currentShapeDir.Rotate90(true)) + Vector2Int.one) / 2 * new Vector2Int(Size - 1, Size - 1) -
        currentShapeDir;

    public static FieldMatrix Active
    {
        get => _active;
        set
        {
            _active = value;
            if (value == null)
                FieldPacksCollection.PropagateFieldMatrixState(FieldScreenState.OnSelectScreen);
        }
    }

    [SerializeField] int _size;

    public int Size
    {
        get => _size;
        set
        {
            _size = value;
            backgroundInputSprite.transform.localScale = new Vector3(_size, _size);
        }
    }

    void OnEnable()
    {
        CollectCells();
        SubscribeCompletionDependency();
        InitCompletion();
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
        shape.shapeObject.SetTargetScale(1f);
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


    readonly MoveTracker _moveTracker = new MoveTracker();
    public void InsertShape()
    {
        if (attachedShape == null) return;
        var move = new ShapeMove(this, attachedShape).Do(); 
        if (move != null)
        {
            var shape = shapesContainer.GetNext();
            if (shape != null)
                AttachShape(shape);
            else
            {
                attachedShape = null;
                var allFilled = true;
                foreach (var cell in _cells)
                    if (cell.OccupiedBy == null)
                    {
                        allFilled = false;
                        break;
                    }

                if (allFilled)
                {
                    Progress.SetComplete(packId, fieldId);
                    SetCompletion(FieldCompletion.Complete);
                    if (!FieldMatrixSerialized.FileExists(packId, fieldId))
                        new FieldMatrixSerialized(this).SaveToFile(packId, fieldId);
                }
            }
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

    void NewActiveFieldHandle()
    {
        if (Active != this) SetState(FieldScreenState.OnSelectScreen);
    }

    public void SetContainer(ShapeContainer container)
    {
        shapesContainer = container;
        var shape = shapesContainer.GetNext();
        if (shape != null)
        {
            currentShapeOffset = shape.Width / 2;
            AttachShape(shape);
        }
    }

    [SerializeField] Transform cellParent;
    public void CreateCells()
    {
        if (this == null ||
            PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.NotAPrefab &&
            PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.NotAPrefab) return;
        
        foreach (var cell in cellParent.GetComponentsInChildren<FieldCell>())
            cell.Destroy();
    
        _cells = new FieldCell[Size, Size];
        for (var x = 0; x < Size; x++)
        {
            for (var y = 0; y < Size; y++)
            {
                _cells[x, y] = FieldCell.Create(this, x, y, cellParent);
            }
        }
        RefreshProjection();
    }

    void CollectCells()
    {
        _cells = new FieldCell[Size, Size];
        foreach (var cell in cellParent.GetComponentsInChildren<FieldCell>())
            _cells[cell.X, cell.Y] = cell;
    }

    public void SetState(FieldScreenState value)
    {
        switch (value)
        {
            case FieldScreenState.Active:
                gameObject.SetActive(true);
                if (shapesContainer == null)
                    FieldMatrixSerialized.Load(packId, fieldId).LoadShapesContainer(this);
                shapesContainer?.SetEnabled(true);
                attachedShape?.shapeObject.gameObject.SetActive(true);
                RefreshProjection();
                Active = this;
                FieldPacksCollection.PropagateFieldMatrixState(FieldScreenState.Disabled, this);
                break;
            case FieldScreenState.Disabled:
                gameObject.SetActive(false);
                break;
            case FieldScreenState.OnSelectScreen:
                gameObject.SetActive(true);
                shapesContainer?.SetEnabled(false);
                attachedShape?.shapeObject.gameObject.SetActive(false);
                SetCellsState(FieldCellState.SelectScreen);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }

        screenState = value;
    }

    public Action<FieldMatrix, FieldCompletion> onCompletionChange;
    public void SetCompletion(FieldCompletion value)
    {
        completion = value;
        switch (value)
        {
            case FieldCompletion.Locked:
                break;
            case FieldCompletion.Unlocked:
                break;
            case FieldCompletion.Complete:
                CompleteTransition();
                FieldPacksCollection.Packs[packId].FieldCompleted();
                Active = null;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
        foreach (var cell in _cells) cell.FieldCompletionStateChangeHandle(value);
        onCompletionChange?.Invoke(this, value);
    }

    void InitCompletion()
    {
        if (Progress.IsComplete(this))
            SetCompletion(FieldCompletion.Complete);
        else if (_leftDependencies.Count == 0) 
            SetCompletion(FieldCompletion.Unlocked);
        else SetCompletion(FieldCompletion.Locked);
    }

    HashSet<FieldMatrix> _leftDependencies = new HashSet<FieldMatrix>();
    void SubscribeCompletionDependency()
    {
        foreach (var fieldMatrix in dependencies)
        {
            if (Progress.IsComplete(fieldMatrix)) continue;
            fieldMatrix.onCompletionChange += DependencyCompletionChangeHandle;
            _leftDependencies.Add(fieldMatrix);
        }
    }

    void DependencyCompletionChangeHandle(FieldMatrix field, FieldCompletion value)
    {
        if (value != FieldCompletion.Complete) return;
        _leftDependencies.Remove(field);
        if (_leftDependencies.Count == 0)
            SetCompletion(FieldCompletion.Unlocked);
    }

    void CompleteTransition()
    {
        completionSprite.transform.localScale = new Vector3(_size, _size, _size);
        completionSprite.SetActive(true);
    }
    
    
    static FieldMatrix _active;

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
        field.backgroundInputSprite.transform.localScale = new Vector3(field.Size, field.Size);
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Active == this) return;
            if (FieldPack.active.packId == packId)
            {
                SetState(FieldScreenState.Active);
            }
            else
            {
                FieldPack.active = FieldPacksCollection.Packs[packId];
            }
        }
        else
        {
            SetCompletion(FieldCompletion.Unlocked);
        }
    }
}