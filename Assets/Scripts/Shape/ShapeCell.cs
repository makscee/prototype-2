using UnityEngine;

public class ShapeCell
{
    FieldMatrix Matrix => shape.Matrix;
    public Shape shape;
    public ShapeCellObject shapeCellObject;
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
        shapeCellObject = ShapeCellObject.Create(localPos, shape);
    }
    
    public bool CanMove(Vector2Int dir, int moves = 1, bool allowPush = true)
    {
        if (Matrix == null) return true;
        var pos = FieldPos;
        for (var i = 1; i <= moves; i++)
        {
            var newPos = FieldPos + dir * i;
            if (!Matrix.CheckIndex(newPos))
            {
                if (moves > (Matrix.size * dir).magnitude || Matrix.CheckIndex(pos))
                    return false;
                continue;
            }

            var fieldCell = Matrix[newPos];
            if (fieldCell.OccupiedBy != null && fieldCell.OccupiedBy != shape)
                return allowPush && fieldCell.OccupiedBy.CanMove(dir, moves - i + 1);
        }

        return true;
    }

    public void Destroy()
    {
        Object.Destroy(shapeCellObject.gameObject);
    }
}