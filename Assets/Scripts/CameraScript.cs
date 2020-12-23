using System;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    Vector2 _targetPosition = Vector2.zero;
    Quaternion _targetRotation = Quaternion.identity;
    float _targetSize = 5;

    Camera _camera;

    void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (FieldMatrix.Active != null)
        {
            if (FieldMatrix.Active.attachedShape != null)
                FollowShape();
            else FollowField();
        } else if (FieldPack.active != null)
        {
            FollowFieldPack();
        }
        ApplyTarget();
    }

    const float LerpMultiplier = 10;
    void ApplyTarget()
    {
        Transform t;
        var targetPosition = new Vector3(_targetPosition.x, _targetPosition.y, transform.position.z);
        var lerpDelta = Time.deltaTime * LerpMultiplier;
        (t = transform).position = Vector3.Lerp(t.position, targetPosition, lerpDelta);
        t.rotation = Quaternion.Lerp(t.rotation, _targetRotation, lerpDelta);
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetSize, lerpDelta);
    }

    void FollowShape()
    {
        var shape = FieldMatrix.Active.attachedShape;
        // _targetPosition = (shape.shapeObject.transform.position + (Vector3)(Vector2)shape.size / 2 + FieldMatrix.current.transform.position) / 2;
        // _targetPosition = FieldMatrix.current.MatrixAttachLocalPosition + FieldMatrix.current.ZeroPos;
        _targetPosition = FieldMatrix.Active.transform.position;
        _targetRotation = shape.RotationQuaternion;
        SetSizeTarget(FieldMatrix.Active.size);
    }

    void FollowField()
    {
        _targetPosition = FieldMatrix.Active.transform.position;
        _targetRotation = FieldMatrix.Active.transform.rotation;
        SetSizeTarget(FieldMatrix.Active.size);
    }

    void FollowFieldPack()
    {
        var aspectRatio = (float) Screen.height / Screen.width;
        SetSizeTarget(FieldPack.active.height);
        _targetPosition = FieldPack.active.centerPosition;
        _targetRotation = Quaternion.identity;
    }

    void SetSizeTarget(float sizeToFit)
    {
        var aspectRatio = (float) Screen.height / Screen.width;
        _targetSize = sizeToFit * aspectRatio / 1.5f;
    }
}