using System;
using UnityEngine;

public class ShapeBuilder : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;

    bool _enabled;
    Vector2Int _curPos;
    static Shape Shape => Matrix.attachedShape;
    static FieldMatrix Matrix => FieldMatrix.Active;
    public static FieldMatrix lastEditedField;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            sr.enabled = value;
            if (value)
                CurPos = Vector2Int.zero;
        }
    }

    public Vector2Int CurPos
    {
        get => _curPos;
        set
        {
            _curPos = value;
            RefreshGameObjectPosition();
        }
    }

    void RefreshGameObjectPosition()
    {
        _curPos.Clamp(Vector2Int.zero, Shape.ShapeRotationSize - Vector2Int.one);
        transform.localPosition = _curPos.FromLocalShapeRotation(Shape).ToField() + Matrix.ZeroPos;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Enabled && Input.GetKeyUp(KeyCode.LeftShift))
        {
            Enabled = false;
            return;
        }

        if (!Enabled && Input.GetKeyDown(KeyCode.LeftShift) && Shape != null)
        {
            Enabled = true;
            Shape.originalRotation = Utils.DirFromCoords(Shape.UpDirection);
            Shape.SetRotation(Shape.UpDirection);
            foreach (var shapeCell in Shape.cells)
                shapeCell.shapeCellObject.InitInsides();
            return;
        }
        if (!Enabled) return;

        Vector2Int dir = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.UpArrow)) dir = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.RightArrow)) dir = Vector2Int.right;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) dir = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) dir = Vector2Int.left;
        ReadInput(dir);
        if (Input.GetKeyDown(KeyCode.Space)) InvertCell();
        if (Input.GetKeyDown(KeyCode.Q)) ResetShape();
#endif
    }

    void ReadInput(Vector2Int dir)
    {
        if (dir == Vector2Int.zero) return;
        var newPos = CurPos + dir;
        
        if (Shape == null) return;
        _curPos = newPos;
        _curPos += Shape.AddCell(newPos.FromLocalShapeRotation(Shape).ToLocalFieldRotation());
        Matrix.MoveAttachedShapeAccordingToDir(Matrix.currentShapeOffset);
        RefreshGameObjectPosition();
        lastEditedField = Matrix;
    }

    void InvertCell()
    {
        var curShape = Matrix.attachedShape;
        if (curShape == null) return;
        var localShapePos = CurPos.FromLocalShapeRotation(curShape).ToLocalFieldRotation();
        var shapeCell = curShape[localShapePos];
        var delta = Vector2Int.zero;
        if (shapeCell == null)
            delta = curShape.AddCell(localShapePos);
        else delta = curShape.RemoveCell(localShapePos);
        _curPos += delta;
        Matrix.MoveAttachedShapeAccordingToDir(Matrix.currentShapeOffset);
        RefreshGameObjectPosition();
        lastEditedField = Matrix;
    }

    void ResetShape()
    {
        var curShape = Matrix.attachedShape;
        if (curShape == null) return;
        curShape.ClearCells();
        curShape.AddCell(Vector2Int.zero);
    }
}