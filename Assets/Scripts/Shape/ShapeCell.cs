using System.Collections.Generic;
using UnityEngine;

public class ShapeCell
{
    FieldMatrix Matrix => shape.Field;
    public Shape shape;
    public ShapeCellObject shapeCellObject;
    public Vector2Int originalPos;
    Vector2Int localPos;

    public Vector2Int FieldPos => shape.pos + localPos;
    public Vector2Int LocalPos
    {
        get => localPos;
        set
        {
            localPos = value;
            shapeCellObject.transform.localPosition = new Vector3(value.x, value.y);
        }
    }

    public ShapeCell(Shape shape, Vector2Int localPos)
    {
        this.shape = shape;
        this.localPos = localPos;
        shapeCellObject = ShapeCellObject.Create(localPos, shape, this);
    }
    
    public bool CanMove(Vector2Int dir, int moves = 1, bool allowPush = true, HashSet<Shape> pushCandidates = null)
    {
        if (Matrix == null) return true;
        if (pushCandidates == null) pushCandidates = new HashSet<Shape>();
        var pos = FieldPos;
        for (var i = 1; i <= moves; i++)
        {
            var newPos = FieldPos + dir * i;
            if (!Matrix.CheckIndex(newPos))
            {
                if (moves > (Matrix.Size * dir).magnitude || Matrix.CheckIndex(pos))
                    return false;
                continue;
            }

            var fieldCell = Matrix[newPos];
            if (fieldCell.OccupiedBy != null && fieldCell.OccupiedBy != shape &&
                !pushCandidates.Contains(fieldCell.OccupiedBy))
            {
                pushCandidates.Add(shape);
                return allowPush && fieldCell.OccupiedBy.CanMove(dir, moves - i + 1, true, pushCandidates);
            }
        }

        return true;
    }

    public void UpdateRotation()
    {
        shapeCellObject.transform.localRotation = 
            Quaternion.AngleAxis(90f * (shape.originalRotation - Utils.DirFromCoords(shape.UpDirection)), Vector3.forward);
    }

    public void Destroy()
    {
        Object.Destroy(shapeCellObject.gameObject);
    }
}