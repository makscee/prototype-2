using System;
using UnityEngine;

public class ShapeObject : MonoBehaviour
{
    public Shape shape;
    public Vector3 CurrentPositionTarget { get; private set; }
    public Vector3 CurrentScaleTarget { get; private set; }

    public static ShapeObject Create(Shape shape)
    {
        var so = Instantiate(Prefabs.Instance.shapeObject).GetComponent<ShapeObject>();
        so.shape = shape;
        var t = so.transform;
        so.CurrentScaleTarget = t.localScale;
        so.CurrentPositionTarget = t.localPosition;
        return so;
    }

    const float LerpMultiplier = 10;
    void Update()
    {
        // if (transform.localScale != _targetScale)
        //     transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.deltaTime * LerpMultiplier);
        // if (transform.localPosition != _targetPosition)
        //     transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPosition, Time.deltaTime * LerpMultiplier);
    }

    public Interpolator<Vector3> SetTargetPosition(Vector3 position)
    {
        var lastTarget = CurrentPositionTarget;
        CurrentPositionTarget = position;

        var interpolator = Animator.Interpolate(lastTarget, position, GlobalConfig.Instance.shapeAnimationTime)
            .PassDelta(v => transform.localPosition += v).Type(InterpolationType.InvSquare).NullCheck(gameObject);
        interpolator.SetOwner(this);
        return interpolator;
    }
    public Interpolator<Vector3> SetTargetScale(Vector3 scale)
    {
        var lastTarget = CurrentScaleTarget;
        CurrentScaleTarget = scale;

        var interpolator = Animator.Interpolate(lastTarget, scale, GlobalConfig.Instance.shapeAnimationTime)
            .PassDelta(v => transform.localScale += v).Type(InterpolationType.InvSquare).NullCheck(gameObject);
        return interpolator;
    }

    public void SetParent(Transform parent)
    {
        Animator.ClearByOwner(this);
        var t = transform;
        t.SetParent(parent);
        CurrentPositionTarget = t.localPosition;
    }

    public void DirectPositionOffset(Vector3 offset)
    {
        CurrentPositionTarget += offset;
        transform.localPosition += offset;
    }
}