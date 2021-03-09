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
            interpolator.whenDone += ShapePlacedEffect;
        }
        return interpolator;
    }

    public void UnableToInsertAnimation()
    {
        var vec = (Vector3)(Vector2)shape.UpDirection * (shape.MaxMoves(shape.UpDirection) + 0.5f);
        transform.localPosition += vec;
        Animator.Interpolate(vec, Vector3.zero, GlobalConfig.Instance.shapeAnimationTime)
            .PassDelta(v => transform.localPosition += v).Type(InterpolationType.Linear).NullCheck(gameObject);
    }
    
    void ShapePlacedEffect()
    {
        shape.Field.onShapePlaced?.Invoke();
        SoundsPlayer.instance.PlayInsertSound();
        CameraScript.instance.transform.position += -(Vector3) (Vector2) shape.UpDirection *
                                                    (GlobalConfig.Instance.cameraShakeAmount *
                                                     shape.Field.transform.lossyScale.x);
        var field = shape.Field;
        var t = field.ShapeSidesThickness;
        t.value += 1.5f;
        field.ShapeSidesThickness = t;
        Animator.Interpolate(0f, 1.5f, GlobalConfig.Instance.sidesThicknessRecoverTime)
            .PassDelta(v =>
            {
                t = field.ShapeSidesThickness;
                t.value -= v;
                field.ShapeSidesThickness = t;
            })
            .Type(InterpolationType.InvSquare);
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

    public void OffsetRotation(float angle)
    {
        transform.rotation = Quaternion.Euler(0, 0, angle + transform.rotation.eulerAngles.z);
        var interpolator = Animator.Interpolate(angle, 0f, GlobalConfig.Instance.shapeAnimationTime)
            .PassDelta(v => transform.rotation = Quaternion.Euler(0, 0, v + transform.rotation.eulerAngles.z))
            .Type(InterpolationType.InvSquare).NullCheck(gameObject);
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

    public void SetEnabled(bool value)
    {
        gameObject.SetActive(value);
        foreach (var shapeCell in shape.cells)
        {
            shapeCell.shapeCellObject.SetEnabled(value);
        }
    }

    [SerializeField] AudioSource openSidesSource, closeSidesSource;
    

    public void PlaySidesSound(bool open)
    {
        if (open)
            openSidesSource.Play();
        else closeSidesSource.Play();
    }
}