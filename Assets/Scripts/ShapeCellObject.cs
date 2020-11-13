using UnityEngine;

public class ShapeCellObject : MonoBehaviour
{
    public static ShapeCellObject Create(int x, int y, Shape shape)
    {
        var sco = Instantiate(Prefabs.Instance.shapeCellObject, shape.shapeObject.transform)
            .GetComponent<ShapeCellObject>();
        sco.transform.localPosition = new Vector3(x, y);
        sco.GetComponent<SpriteRenderer>().color = shape.color;
        return sco;
    }
}