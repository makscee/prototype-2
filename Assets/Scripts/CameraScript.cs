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
        var configSpeed = GlobalConfig.Instance.cameraFollowSpeed; 
        if (_speed < configSpeed)
            _speed = Mathf.Min(configSpeed, _speed + Time.deltaTime * configSpeed);
        RefreshTarget();
        ApplyTarget();
    }

    int _curTarget = -1;
    void RefreshTarget()
    {
        var newTarget = -1;
        if (FieldMatrix.Active != null)
        {
            if (FieldMatrix.Active.attachedShape != null)
                newTarget = 0;
            else newTarget = 1;
        } else if (FieldPack.active != null) newTarget = 2;

        if (newTarget != _curTarget)
        {
            _speed = 0f;
            _curTarget = newTarget;
        }
        switch (_curTarget)
        {
            case 0:
                FollowShape();
                break;
            case 1:
                FollowField();
                break;
            case 2:
                FollowFieldPack();
                break;
        }
    }

    float _speed;
    void ApplyTarget()
    {
        Transform t;
        var targetPosition = new Vector3(_targetPosition.x, _targetPosition.y, transform.position.z);
        var lerpDelta = Time.deltaTime * _speed;
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
        _targetRotation = shape.RotationQuaternion * matrix.transform.rotation;
        SetSizeTarget(matrix.Size * t.lossyScale.x);
    }

    void FollowField()
    {
        var matrix = FieldMatrix.Active;
        var t = matrix.transform;
        _targetPosition = t.position;
        _targetRotation = t.rotation;
        SetSizeTarget(matrix.Size * t.lossyScale.x * GlobalConfig.Instance.cameraFieldSizeMult);
    }

    void FollowFieldPack()
    {
        var fp = FieldPack.active;
        var t = fp.transform;
        SetSizeTarget(fp.size * fp.transform.localScale.x * GlobalConfig.Instance.cameraFPSizeMult);
        _targetPosition = Vector2.zero;
        _targetRotation = t.rotation;
    }

    void SetSizeTarget(float sizeToFit)
    {
        var aspectRatio = (float) Screen.height / Screen.width;
        if (Screen.width > Screen.height) 
            aspectRatio = (float) Screen.width / Screen.height;
        _targetSize = sizeToFit * aspectRatio;
    }
}