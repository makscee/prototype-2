using System;
using UnityEngine;

public class ShapeContainerObject : MonoBehaviour
{
    public FieldMatrix matrix;
    public ShapeContainer container;
    
    int _lastCount = -1;

    Vector2 _targetPosition;
    Quaternion _targetRotation;

    void Awake()
    {
        GlobalConfig.onValidate += OnValidate;
    }

    const float LerpMultiplier = 10;
    void Update()
    {
        var cnt = container.shapes.Count - container.currentIndex;
        if (_lastCount != cnt)
        {
            RefreshShapesPlacement();
            _lastCount = cnt;
        }


        var shape = matrix.attachedShape;
        if (shape == null) return;

        // transform.localPosition = shapeObject.transform.localPosition +
        //                           new Vector3(offsetX, offsetY) +
        //                           (Vector3) (Vector2) (shape.size - Vector2Int.one) / 2f;
        // transform.localRotation = shape.RotationQuaternion;
        _targetPosition = matrix.MatrixAttachLocalPosition + matrix.ZeroPos +
                                  new Vector2(GlobalConfig.Instance.containerOffsetX, GlobalConfig.Instance.containerOffsetY).Rotate90(true,
                                      Utils.DirFromCoords(matrix.currentShapeDir));
        _targetRotation = Quaternion.AngleAxis(-90f * Utils.DirFromCoords(matrix.currentShapeDir), Vector3.forward);
        
        transform.localPosition =
            Vector2.Lerp(transform.localPosition, _targetPosition, Time.deltaTime * LerpMultiplier);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, _targetRotation, Time.deltaTime * LerpMultiplier);
    }

    void OnValidate()
    {
        if (container != null && Application.isPlaying)
            RefreshShapesPlacement();
    }

    void RefreshShapesPlacement()
    {
        var curY = 0f;
        for (var i = container.currentIndex; i < container.shapes.Count; i++)
        {
            var shape = container.shapes[i];
            curY -= shape.Height * GlobalConfig.Instance.containerSizeScale + GlobalConfig.Instance.containerPaddingY;
            var curX = -shape.Width * GlobalConfig.Instance.containerSizeScale;
            shape.shapeObject.SetTargetPosition(
                new Vector3(curX, curY));
            shape.shapeObject.SetTargetScale(GlobalConfig.Instance.containerScale);
        }
    }
}