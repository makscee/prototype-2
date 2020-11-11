using UnityEngine;

public class ShapeObject : MonoBehaviour
{
    public Shape shape;

    public static ShapeObject Create(Shape shape)
    {
        var go = Instantiate(Prefabs.Instance.shapeObject);
        return go.GetComponent<ShapeObject>();
    }
}