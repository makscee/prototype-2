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
    
    public bool CanMove(Vector2Int dir, int moves = 1, bool allowPush = true, Dictionary<Shape, int> pushCandidates = null)
    {
        if (Matrix == null) return true;
        if (pushCandidates == null) pushCandidates = new Dictionary<Shape, int>();
        var pos = FieldPos;
        for (var i = 1; i <= moves; i++)
        {
            var newPos = FieldPos + dir * i;
            if (!Matrix.CheckIndex(newPos))
            {
                if (moves > Matrix.Size || Matrix.CheckIndex(pos))
                    return false;
                continue;
            }

            var fieldCell = Matrix[newPos];
            var occupier = fieldCell.OccupiedBy;
            var neededMoves = moves - i + 1;
            
            if (occupier == null || occupier == shape || 
                pushCandidates.ContainsKey(occupier) && pushCandidates[occupier] >= neededMoves) continue;
            
            if (!pushCandidates.ContainsKey(occupier)) pushCandidates.Add(occupier, neededMoves);
            else pushCandidates[occupier] = neededMoves;
            return allowPush && fieldCell.OccupiedBy.CanMove(dir, neededMoves, true, pushCandidates);
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