using UnityEngine;

public class ShapeCell
{
    public FieldMatrix matrix;
    public Shape shape;
    public ShapeCellObject shapeCellObject;
    public int localX, localY;
    public int FieldX => shape.X + localX;
    public int FieldY => shape.Y + localY;

    public ShapeCell(Shape shape, int x, int y)
    {
        this.shape = shape;
        localX = x;
        localY = y;
        shapeCellObject = ShapeCellObject.Create(x, y, shape);
    }
    
    public bool CanMove(Vector2Int dir)
    {
        if (matrix == null) return true;
        var fieldCell = matrix[FieldX + dir.x, FieldY + dir.y];
        return fieldCell != null && (fieldCell.occupiedBy == null || fieldCell.occupiedBy == shape);
    }
}