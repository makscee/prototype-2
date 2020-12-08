using UnityEngine;

public class ShapeCellObject : MonoBehaviour
{
    public SpriteRenderer sr;
    public Vector2 shaderPosition, shaderSize;
    static readonly int PositionProperty = Shader.PropertyToID("position");
    static readonly int SizeProperty = Shader.PropertyToID("size");

    public static ShapeCellObject Create(Vector2Int pos, Shape shape)
    {
        var sco = Instantiate(Prefabs.Instance.shapeCellObject, shape.shapeObject.transform)
            .GetComponent<ShapeCellObject>();
        sco.transform.localPosition = (Vector2)pos;
        sco.sr = sco.GetComponent<SpriteRenderer>();
        sco.sr.color = shape.color;
        return sco;
    }

    public void SetMaterial(Material material)
    {
        sr.material = material;
        sr.material.SetVector(PositionProperty, shaderPosition);
        sr.material.SetVector(SizeProperty, shaderSize);
    }
}