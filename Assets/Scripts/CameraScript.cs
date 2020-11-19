using System;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    Vector2 _targetPosition;
    Quaternion _targetRotation;


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

    const float Multiplier = 10;
    void ApplyTarget()
    {
        Transform t;
        var targetPosition = new Vector3(_targetPosition.x, _targetPosition.y, transform.position.z);
        (t = transform).position = Vector3.Lerp(t.position, targetPosition, Time.deltaTime * Multiplier);
        t.rotation = Quaternion.Lerp(t.rotation, _targetRotation, Time.deltaTime * Multiplier);
    }

    void FollowShape()
    {
        var shape = FieldMatrix.current.attachedShape;
        _targetPosition = (shape.shapeObject.transform.position + (Vector3)(Vector2)shape.size / 2 + FieldMatrix.current.transform.position) / 2;
        _targetRotation = Quaternion.AngleAxis(-90f * Utils.DirFromCoords(shape.UpDirection), Vector3.forward);
    }

    void FollowField()
    {
        _targetPosition = FieldMatrix.current.transform.position;
        _targetRotation = FieldMatrix.current.transform.rotation;
    }
}