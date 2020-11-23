using System;
using UnityEngine;

public class ShapeObject : MonoBehaviour
{
    public Shape shape;
    public Vector3 targetScale = Vector3.one, targetPosition;

    public static ShapeObject Create(Shape shape)
    {
        var so = Instantiate(Prefabs.Instance.shapeObject).GetComponent<ShapeObject>();
        so.shape = shape;
        return so;
    }

    const float LerpMultiplier = 10;
    void Update()
    {
        if (transform.localScale != targetScale)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * LerpMultiplier);
        if (transform.localPosition != targetPosition)
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * LerpMultiplier);
    }

    public void SetPosition(Vector3 pos)
    {
        transform.localPosition = pos;
        targetPosition = pos;
    }

    public void SetPositionDelta(Vector3 deltaPos)
    {
        transform.localPosition += deltaPos;
        targetPosition += deltaPos;
    }
}