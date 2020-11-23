using System;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static CameraScript current; 
    
    Vector2 _targetPosition;
    Quaternion _targetRotation;

    void Awake()
    {
        current = this;
    }
    void Update()
    {
        if (FieldMatrix.current != null)
        {
            if (FieldMatrix.current.attachedShape != null)
                FollowShape();
            else FollowField();
        }
        ApplyTarget();
    }

    const float LerpMultiplier = 10;
    void ApplyTarget()
    {
        Transform t;
        var targetPosition = new Vector3(_targetPosition.x, _targetPosition.y, transform.position.z);
        (t = transform).position = Vector3.Lerp(t.position, targetPosition, Time.deltaTime * LerpMultiplier);
        t.rotation = Quaternion.Lerp(t.rotation, _targetRotation, Time.deltaTime * LerpMultiplier);
    }

    void FollowShape()
    {
        var shape = FieldMatrix.current.attachedShape;
        // _targetPosition = (shape.shapeObject.transform.position + (Vector3)(Vector2)shape.size / 2 + FieldMatrix.current.transform.position) / 2;
        // _targetPosition = FieldMatrix.current.MatrixAttachLocalPosition + FieldMatrix.current.ZeroPos;
        _targetPosition = FieldMatrix.current.transform.position;
        _targetRotation = shape.RotationQuaternion;
    }

    void FollowField()
    {
        _targetPosition = FieldMatrix.current.transform.position;
        _targetRotation = FieldMatrix.current.transform.rotation;
    }
}