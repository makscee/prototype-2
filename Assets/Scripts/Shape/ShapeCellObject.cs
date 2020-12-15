using UnityEngine;

public class ShapeCellObject : MonoBehaviour
{
    public ShapeCell cell;
    public static ShapeCellObject Create(Vector2Int pos, Shape shape, ShapeCell cell)
    {
        var sco = Instantiate(Prefabs.Instance.shapeCellObject, shape.shapeObject.transform)
            .GetComponent<ShapeCellObject>();
        sco.transform.localPosition = (Vector2)pos;
        sco.cell = cell;
        return sco;
    }
}