using System;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static CameraScript instance;
    Vector2 _targetPosition = Vector2.zero;
    Quaternion _targetRotation = Quaternion.identity;
    float _targetSize = 5;

    Camera _camera;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        instance = this;
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
    void ApplyTarget()
    {
        var config = GlobalConfig.Instance;
        Transform t;
        var targetPosition = new Vector3(_targetPosition.x, _targetPosition.y, transform.position.z);
        var lerpDelta = Time.deltaTime * config.cameraFollowSpeed;
        (t = transform).position = Vector3.Lerp(t.position, targetPosition, lerpDelta);
        t.rotation = Quaternion.Lerp(t.rotation, _targetRotation, lerpDelta);
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetSize, lerpDelta);
    }

    void FollowShape()
    {
        var matrix = FieldMatrix.Active;
        var shape = matrix.attachedShape;
        // _targetPosition = (shape.shapeObject.transform.position + (Vector3)(Vector2)shape.size / 2 + FieldMatrix.current.transform.position) / 2;
        // _targetPosition = FieldMatrix.current.MatrixAttachLocalPosition + FieldMatrix.current.ZeroPos;
        var t = matrix.transform;
        _targetPosition = t.position;
        _targetRotation = shape.RotationQuaternion;
        SetSizeTarget(matrix.Size * t.lossyScale.x);
    }

    void FollowField()
    {
        var matrix = FieldMatrix.Active;
        var t = matrix.transform;
        _targetPosition = t.position;
        _targetRotation = t.rotation;
        SetSizeTarget(matrix.Size * t.lossyScale.x);
    }

    void FollowFieldPack()
    {
        SetSizeTarget(FieldPack.active.Height * 1.5f);
        _targetPosition = Vector2.zero;
        _targetRotation = Quaternion.identity;
    }

    void SetSizeTarget(float sizeToFit)
    {
        var aspectRatio = (float) Screen.height / Screen.width;
        if (Screen.width > Screen.height) 
            aspectRatio = (float) Screen.width / Screen.height;
        _targetSize = sizeToFit * aspectRatio;
    }
}