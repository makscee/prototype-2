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

    public ShapeCell(Shape shape, int x, int y)
    {
        this.shape = shape;
        localPos = new Vector2Int(x, y);
        shapeCellObject = ShapeCellObject.Create(x, y, shape);
    }
    
    public bool CanMove(Vector2Int dir)
    {
        if (Matrix == null) return true;
        var pos = FieldPos;
        var newPos = FieldPos + dir;
        if (!Matrix.CheckIndex(newPos))
            return !Matrix.CheckIndex(pos);

        var fieldCell = Matrix[newPos];
        return fieldCell.OccupiedBy == null || fieldCell.OccupiedBy == shape || fieldCell.OccupiedBy.CanMove(dir);
    }

    Vector2Int _lastPos = new Vector2Int(-1, -1);
}