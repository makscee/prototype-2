using UnityEngine;

public class ShapeCellObject : MonoBehaviour
{
    SpriteRenderer _sr;
    public Vector2 shaderPosition, shaderSize;
    static readonly int PositionProperty = Shader.PropertyToID("_Position");
    static readonly int SizeProperty = Shader.PropertyToID("_Size");

    public static ShapeCellObject Create(Vector2Int pos, Shape shape)
    {
        var sco = Instantiate(Prefabs.Instance.shapeCellObject, shape.shapeObject.transform)
            .GetComponent<ShapeCellObject>();
        sco.transform.localPosition = (Vector2)pos;
        sco._sr = sco.GetComponent<SpriteRenderer>();
        sco._sr.color = shape.color;
        return sco;
    }

    public void SetMaterial(Material material)
    {
        _sr.material = material;
        _sr.material.SetVector(PositionProperty, shaderPosition);
        _sr.material.SetVector(SizeProperty, shaderSize);
    }
}