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

    public Interpolator<Vector3> SetTargetPosition(Vector3 position, bool shakeCamera = false)
    {
        var lastTarget = CurrentPositionTarget;
        CurrentPositionTarget = position;

        var interpolator = Animator.Interpolate(lastTarget, position, GlobalConfig.Instance.shapeAnimationTime)
            .PassDelta(v => transform.localPosition += v).Type(InterpolationType.Linear).NullCheck(gameObject);
        interpolator.SetOwner(this);
        if (shakeCamera)
        {
            interpolator.whenDone += ShapePlaceCameraShake;
        }
        return interpolator;
    }
    
    void ShapePlaceCameraShake()
    {
        CameraScript.instance.transform.position += -(Vector3)(Vector2)shape.UpDirection * GlobalConfig.Instance.cameraShakeAmount;
    }
    public Interpolator<Vector3> SetTargetScale(float scale)
    {
        var lastTarget = CurrentScaleTarget;
        var scaleV = new Vector3(scale, scale, scale);
        CurrentScaleTarget = scaleV;

        var interpolator = Animator.Interpolate(lastTarget, scaleV, GlobalConfig.Instance.shapeAnimationTime)
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