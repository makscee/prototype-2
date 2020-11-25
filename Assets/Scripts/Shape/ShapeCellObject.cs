using UnityEngine;

public class ShapeCellObject : MonoBehaviour
{
    public static ShapeCellObject Create(Vector2Int pos, Shape shape)
    {
        var sco = Instantiate(Prefabs.Instance.shapeCellObject, shape.shapeObject.transform)
            .GetComponent<ShapeCellObject>();
        sco.transform.localPosition = (Vector2)pos;
        sco.GetComponent<SpriteRenderer>().color = shape.color;
        return sco;
    }
}