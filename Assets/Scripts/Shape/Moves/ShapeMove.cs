using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShapeMove
{
    public Vector2Int direction;
    public int offset;
    public readonly Shape mover;
    Dictionary<Shape, Vector2Int> _pushes = new Dictionary<Shape, Vector2Int>();

    public ShapeMove(FieldMatrix matrix, Shape shape)
    {
        direction = shape.UpDirection;
        mover = shape;
        offset = matrix.currentShapeOffset;
    }

    public ShapeMove Do()
    {
        if (_pushes.Count > 0)
            throw new Exception("Move was already made");
        var height = mover.Height;
        if (!mover.CanMove(direction, height))
            return null;

        var moves = mover.MaxMoves(direction);
        var toPush = new Dictionary<Shape, float>();
        toPush.Add(mover, moves);
        for (var i = 0; i < moves; i++)
            foreach (var pushed in mover.Move(direction))
            {
                if (!toPush.ContainsKey(pushed)) toPush.Add(pushed, 0);
                toPush[pushed] += 1f;
            }
        foreach (var f in toPush)
        {
            _pushes.Add(f.Key, direction * Mathf.RoundToInt(f.Value));
        }

        float totalPush = moves;

        foreach (var shape in toPush.Keys.ToArray())
        {
            var pushLength = toPush[shape];
            var allowedDeltaFrom = 1f - pushLength / totalPush;
            var shake = shape == mover;
            shape.shapeObject.SetTargetPosition(shape.shapeObject.CurrentPositionTarget +
                                                pushLength * (Vector3) (Vector2) direction, shake)
                .AllowedDelta(allowedDeltaFrom, 1f);
        }
        return this;
    }

    public Shape Undo()
    {
        var maxDelta = 0;
        var toPush = new Dictionary<Shape, float>();
        foreach (var push in _pushes)
        {
            var shape = push.Key;
            var delta = push.Value;
            shape.Translate(shape.pos - delta);
            maxDelta = Mathf.Max(maxDelta, Mathf.RoundToInt(delta.magnitude));
            toPush.Add(shape, delta.magnitude);
        }
        
        float totalPush = maxDelta;
        foreach (var shape in toPush.Keys.ToArray())
        {
            var pushLength = toPush[shape];
            var allowedDeltaTo = totalPush / pushLength;
            shape.shapeObject.SetTargetPosition(shape.shapeObject.CurrentPositionTarget +
                                                -pushLength * (Vector3) (Vector2) direction)
                .AllowedDelta(0f, allowedDeltaTo);
        }
        return mover;
    }
}