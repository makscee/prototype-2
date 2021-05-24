using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.EventSystems;

public class FieldMatrix : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject backgroundInputSprite, unlockedSprite;
    public PatternMaterialProvider completionSprite;
    public PatternMaterialProvider cellsMaterialProvider;
    public int packId, fieldId, unlockIndex;
    [Range(-5, 5)]
    public int packPositionX, packPositionY;
    public ShapeContainer shapesContainer;
    public Shape attachedShape;
    public bool isComplete { get; private set; }

    public FieldScreenState screenState;    
    public FieldCompletion completion;

    public FieldMatrix[] dependencies1, dependencies2;
    public Vector2 ZeroPos => new Vector2(-(Size - 1) / 2f, -(Size - 1) / 2f);
    public FieldPack Pack => FieldPacksCollection.Packs == null || FieldPacksCollection.Packs.Length == 0 ? GetComponentInParent<FieldPack>() : FieldPacksCollection.Packs[packId];

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
            var isNull = value == null;
            if (isNull)
            {
                FieldPacksCollection.PropagateFieldMatrixState(FieldScreenState.OnSelectScreen);
                SoundsPlayer.instance.EnableSelectScreenTheme(true);
                FieldPack.active.SetHoveredByUnlockIndex();
            }
            TouchInputObject.SetEnabled(!isNull);
            // GameManager.instance.keysHintText.SetActive(!isNull);
            GameManager.instance.clearProgressButton.SetActive(isNull);
        }
    }

    [SerializeField] int size;

    public int Size
    {
        get => size;
        set
        {
            size = value;
            backgroundInputSprite.transform.localScale = new Vector3(size, size);
        }
    }

    
    FloatTarget _shapeSidesThickness;
    public FloatTarget ShapeSidesThickness
    {
        get => _shapeSidesThickness;
        set
        {
            _shapeSidesThickness = value;
            foreach (var shapeCell in shapesContainer.shapes.SelectMany(shape => shape.cells))
                shapeCell.shapeCellObject.sidesContainer.RefreshSides();
        }
    }
    
    public Action onShapePlaced;

    void Awake()
    {
        prevX = packPositionX;
        prevY = packPositionY;
        if (Application.isPlaying)
        {
            CollectCells();
            SubscribeCompletionDependency();
            InitCompletion();
            // Pack.PlaceField(this);
            SetScreenState(FieldScreenState.OnSelectScreen);
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
        shape.Field = this;
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
        if (attachedShape == null || isComplete) return;
        var move = new ShapeMove(this, attachedShape).Do();
        if (move != null)
        {
            SoundsPlayer.instance.PlayInsertStartSound();
            _moveTracker.AddMove(move);

            var shape = shapesContainer.GetNext();
            if (IsAllFilled())
            {
                CompleteLevel();
                return;
            }

            if (shape != null)
                AttachShape(shape);
            else
            {
                attachedShape = null;
                shapesContainer
                    .InsertAtCurrent(ShapeSerialized.CreateFromString(new[] {"*"})
                    .Deserialize());
                AttachShape(shapesContainer.GetNext());
            }
        }
        else
        {
            attachedShape.shapeObject.UnableToInsertAnimation();
        }
    }

    void CompleteLevel()
    {
        Progress.SetComplete(packId, fieldId);
        if (ShapeBuilder.lastEditedField == this)
        {
            new FieldMatrixSerialized(this).SaveToFile(packId, fieldId);
            ShapeBuilder.lastEditedField = null;
        }
        CompleteTransition();
    }

    bool IsAllFilled()
    {
        foreach (var cell in _cells)
            if (cell.OccupiedBy == null)
                return false;
        return true;
    }

    public void Undo()
    {
        if (_moveTracker.Moves == 0 || isComplete) return;
        if (attachedShape != null)
            shapesContainer.ReturnPrevious();
        var direction = _moveTracker.Last.direction;
        currentShapeOffset = _moveTracker.Last.offset;
        currentShapeDir = direction;
        var shape = _moveTracker.Undo();
        AttachShape(shape);
        SoundsPlayer.instance.PlayUndoSound();
    }

    public void MoveAttachedShape(bool right)
    {
        if (attachedShape == null || isComplete) return;
        var curOffset = currentShapeOffset;
        if (right)
        {
            if (curOffset < MaxShapeOffset)
            {
                MoveAttachedShapeAccordingToDir(curOffset + 1);
                SoundsPlayer.instance.PlayMoveAttachedSound(false);
            }
            else
            {
                var upDir = Utils.DirFromCoords(attachedShape.UpDirection);
                switch (upDir)
                {
                    case 0:
                        attachedShape.shapeObject.DirectPositionOffset(new Vector3(0, attachedShape.Height - 1));
                        break;
                    case 1:
                        attachedShape.shapeObject.DirectPositionOffset(new Vector3(0, attachedShape.Width - 1));
                        break;
                    case 2:
                        attachedShape.shapeObject.DirectPositionOffset(new Vector3(0, attachedShape.Height - 1));
                        break;
                    case 3:
                        attachedShape.shapeObject.DirectPositionOffset(new Vector3(0, attachedShape.Width - 1));
                        break;
                }
                attachedShape.SetRotation(attachedShape.UpDirection.Rotate90(false));
                attachedShape.shapeObject.OffsetRotation(-90f);
                MoveAttachedShapeAccordingToDir(0);
                SoundsPlayer.instance.PlayMoveAttachedRotateSound(false);
            }
        }
        else
        {
            if (curOffset > 0)
            {
                MoveAttachedShapeAccordingToDir(curOffset - 1);
                SoundsPlayer.instance.PlayMoveAttachedSound(true);
            }
            else
            {
                var upDir = Utils.DirFromCoords(attachedShape.UpDirection);
                switch (upDir)
                {
                    case 0:
                        attachedShape.shapeObject.DirectPositionOffset(new Vector3(attachedShape.Width - 1, 0));
                        break;
                    case 1:
                        attachedShape.shapeObject.DirectPositionOffset(new Vector3(attachedShape.Height - 1, 0));
                        break;
                    case 2:
                        attachedShape.shapeObject.DirectPositionOffset(new Vector3(attachedShape.Width - 1, 0));
                        break;
                    case 3:
                        attachedShape.shapeObject.DirectPositionOffset(new Vector3(attachedShape.Height - 1, 0));
                        break;
                }
                attachedShape.SetRotation(attachedShape.UpDirection.Rotate90(true));
                attachedShape.shapeObject.OffsetRotation(90f);
                MoveAttachedShapeAccordingToDir(MaxShapeOffset);
                SoundsPlayer.instance.PlayMoveAttachedRotateSound(true);
            }
        }

        RefreshProjection();
    }

    public void RefreshProjection()
    {
        if (attachedShape == null)
        {
            SetCellsState(FieldProjectionState.Empty);
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
            var val = FieldProjectionState.Empty;
            if (cell.X >= xMin && cell.X <= xMax && cell.Y >= yMin && cell.Y <= yMax)
                val = FieldProjectionState.ShapeProjectionTrail;
            cell.SetProjectionState(val);
        }

        var maxMoveVec = maxMoves * currentShapeDir;
        foreach (var shapeCell in attachedShape.cells)
        {
            var pos = shapeCell.LocalPos + attachedShape.pos + maxMoveVec;
            if (CheckIndex(pos))
                this[pos].SetProjectionState(FieldProjectionState.ShapeProjection);
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
    [SerializeField] bool createCells;
    [SerializeField] bool initFieldFromLevel;
    [SerializeField] int prevX, prevY;

    void OnValidate()
    {
#if UNITY_EDITOR
        if (createCells)
        {
            EditorApplication.delayCall += CreateCells;
            createCells = false;
        }
        if (initFieldFromLevel)
        {
            EditorApplication.delayCall += InitFieldFromLevel;
            initFieldFromLevel = false;
        }

        if (prevX != packPositionX || prevY != packPositionY)
        {
            prevX = packPositionX;
            prevY = packPositionY;
            Pack?.PlaceFields();
        }
#endif
    }

    void InitFieldFromLevel()
    {
        size = FieldMatrixSerialized.Load(packId, fieldId).size;
        CreateCells();
#if UNITY_EDITOR
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
        EditorUtility.SetDirty(gameObject);
        EditorUtility.SetDirty(this);
#endif
    }

    public void CreateCells()
    {
        // if (this == null ||
        //     PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.NotAPrefab &&
        //     PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.NotAPrefab) return;
        
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

        if (Application.isPlaying)
        {
            RefreshProjection();
            foreach (var cell in _cells)
                cell.RefreshTargets();
            MoveAttachedShapeAccordingToDir(currentShapeOffset);
        }

        var scale = 1f / Size;
        transform.localScale = new Vector3(scale, scale, scale);
        backgroundInputSprite.transform.localScale = new Vector3(Size, Size);
        unlockedSprite.transform.localScale = new Vector3(Size, Size);
    }

    void CollectCells()
    {
        _cells = new FieldCell[Size, Size];
        foreach (var cell in cellParent.GetComponentsInChildren<FieldCell>())
            _cells[cell.X, cell.Y] = cell;
    }

    public void SetScreenState(FieldScreenState value)
    {
        switch (value)
        {
            case FieldScreenState.Active:
                unlockedSprite.SetActive(false);
                gameObject.SetActive(true);
                if (shapesContainer == null)
                    FieldMatrixSerialized.Load(packId, fieldId).LoadShapesContainer(this);
                shapesContainer?.SetEnabled(true);
                attachedShape?.shapeObject.gameObject.SetActive(true);
                RefreshProjection();
                Active = this;
                FieldPacksCollection.PropagateFieldMatrixState(FieldScreenState.Disabled, this);
                SoundsPlayer.instance.EnableSelectScreenTheme(false);
                SoundsPlayer.instance.PlayFieldOpen();
                SetHovered(false);
                break;
            case FieldScreenState.Disabled:
                gameObject.SetActive(false);
                SetHovered(false);
                break;
            case FieldScreenState.OnSelectScreen:
                if (completion == FieldCompletion.Unlocked)
                    unlockedSprite.SetActive(true);
                gameObject.SetActive(true);
                shapesContainer?.SetEnabled(false);
                attachedShape?.shapeObject.gameObject.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
        screenState = value;
        foreach (var cell in _cells)
        {
            cell.RefreshTargets();
        }
    }

    public Action<FieldMatrix, FieldCompletion> onCompletionChange;
    public void SetCompletion(FieldCompletion value)
    {
        completion = value;
        isComplete = value == FieldCompletion.Complete;
        switch (value)
        {
            case FieldCompletion.Locked:
                unlockedSprite.SetActive(false);
                break;
            case FieldCompletion.Unlocked:
                unlockedSprite.SetActive(true);
                completionSprite.gameObject.SetActive(false);
                break;
            case FieldCompletion.Complete:
                unlockedSprite.SetActive(false);
                completionSprite.transform.localScale = new Vector3(size, size, size);
                completionSprite.transform.GetChild(0).localScale =
                    GetComponentInParent<FieldPack>().transform.localScale;
                completionSprite.gameObject.SetActive(true);
                shapesContainer?.SetEnabled(false);
                cellParent.gameObject.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
        foreach (var cell in _cells) cell.RefreshTargets();
        onCompletionChange?.Invoke(this, value);
    }

    void InitCompletion()
    {
        if (Progress.IsComplete(this))
        {
            SetCompletion(FieldCompletion.Complete);
            Pack.FieldCompleted();
        }
        else if (_leftDependencies1.Count == 0 ||
                 dependencies2 != null && dependencies2.Length != 0 && _leftDependencies2.Count == 0)
            SetCompletion(FieldCompletion.Unlocked);
        else SetCompletion(FieldCompletion.Locked);
    }

    readonly HashSet<FieldMatrix> _leftDependencies1 = new HashSet<FieldMatrix>();
    readonly HashSet<FieldMatrix> _leftDependencies2 = new HashSet<FieldMatrix>();

    void SubscribeCompletionDependency()
    {
        foreach (var fieldMatrix in dependencies1)
        {
            if (Progress.IsComplete(fieldMatrix)) continue;
            fieldMatrix.onCompletionChange += DependencyCompletionChangeHandle;
            _leftDependencies1.Add(fieldMatrix);
        }
        foreach (var fieldMatrix in dependencies2)
        {
            if (Progress.IsComplete(fieldMatrix)) continue;
            fieldMatrix.onCompletionChange += DependencyCompletionChangeHandle;
            _leftDependencies2.Add(fieldMatrix);
        }
    }

    void DependencyCompletionChangeHandle(FieldMatrix field, FieldCompletion value)
    {
        if (value != FieldCompletion.Complete || completion != FieldCompletion.Locked) return;
        _leftDependencies1.Remove(field);
        _leftDependencies2.Remove(field);
        if (dependencies1 != null && dependencies1.Length != 0 && _leftDependencies1.Count == 0 ||
            dependencies2 != null && dependencies2.Length != 0 && _leftDependencies2.Count == 0)
            Animator.Invoke(() => SetCompletion(FieldCompletion.Unlocked)).In(1f);
    }

    public void CompleteTransition()
    {
        isComplete = true;
        completionSprite.balance = 1f;
        completionSprite.balanceTarget = 1f;
        completionSprite.SetShaderProperties();
        var config = GlobalConfig.Instance;
        SequenceFramework.New
            .Delay((config.sidesThicknessRecoverTime + config.fieldCompleteTransitionAnimationTime) * 2)
            .FieldSet(this).ShapeSidesThicknessRandomBlink()
            .Chain(config.fieldCompleteTransitionAnimationTime).Field.ShapeSidesThicknessChange(1.5f)
            .Chain().Lambda(SoundsPlayer.instance.PlayFieldComplete)
            .Chain().Field.CellsPatternBalanceChange(1f)
            .Chain().Field.CompletionSpriteBalanceChange(0.5f)
            .With().Lambda(() =>
            {
                SetCompletion(FieldCompletion.Complete);
                FieldPacksCollection.Packs[packId].FieldCompleted();
                Active = null;
            })
            .AnyKeyFastForward();
        // SequenceFramework.New.Delay(config.sidesThicknessRecoverTime * 2).FieldSet(this)
        //     .ShapeSidesThicknessChange(1.5f);
    }
    
    
    static FieldMatrix _active;

    void SetCellsState(FieldProjectionState state)
    {
        foreach (var cell in _cells) cell.SetProjectionState(state);
    }

    public bool hovered;
    public Action<bool> onHoveredChange;
    public void SetHovered(bool value)
    {
        if (screenState != FieldScreenState.OnSelectScreen && value || hovered == value) return;
        hovered = value;
        if (value)
            foreach (var field in FieldPacksCollection.Packs[packId].fields)
                if (field != this && field.hovered)
                    field.SetHovered(false);
        onHoveredChange?.Invoke(value);
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

    public Action onClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        onClick?.Invoke();
        if (Active == this || completion == FieldCompletion.Locked) return;
        if (FieldPack.active.packId == packId && completion == FieldCompletion.Unlocked)
        {
            SetScreenState(FieldScreenState.Active);
            return;
        }
        if (completion != FieldCompletion.Locked && FieldPack.active.packId != packId)
        {
            FieldPacksCollection.Packs[packId].Activate();
        }
    }
}