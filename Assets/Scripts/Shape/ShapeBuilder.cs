using System;
using UnityEngine;

public class ShapeBuilder : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;

    bool _enabled;
    Vector2Int _curPos;
    static Shape Shape => Matrix.attachedShape;
    static FieldMatrix Matrix => FieldMatrix.current;

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
        transform.localPosition = CurPos.FromLocalShapeRotation(Shape).ToField() + Matrix.ZeroPos;
    }

    void Update()
    {
        if (Enabled && Input.GetKeyUp(KeyCode.LeftShift))
        {
            Enabled = false;
            return;
        }

        if (!Enabled && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Enabled = true;
            return;
        }

        Vector2Int dir = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.UpArrow)) dir = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.RightArrow)) dir = Vector2Int.right;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) dir = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) dir = Vector2Int.left;
        ReadInput(dir);
        if (Input.GetKeyDown(KeyCode.Space)) InvertCell();
    }

    void ReadInput(Vector2Int dir)
    {
        if (dir == Vector2Int.zero) return;
        var newPos = CurPos + dir;
        
        var curShape = Matrix.attachedShape;
        if (curShape == null) return;
        CurPos = newPos;
        CurPos += curShape.AddCell(newPos.FromLocalShapeRotation(curShape).ToLocalFieldRotation());
        Matrix.MoveAttachedShapeAccordingToDir(Matrix.currentShapeOffset);
        RefreshGameObjectPosition();
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
        CurPos += delta;
        Matrix.MoveAttachedShapeAccordingToDir(Matrix.currentShapeOffset);
        RefreshGameObjectPosition();
    }
}