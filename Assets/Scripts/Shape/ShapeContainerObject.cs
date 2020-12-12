using UnityEngine;

public class ShapeContainerObject : MonoBehaviour
{
    public FieldMatrix matrix;
    public ShapeContainer container;
    const float xMult = -1.3f, yMult = -0.9f, offsetX = -0.5f, offsetY = -1.4f;
    const float scaleMult = 2.38f, xAccum = 1.1f, yAccum = 1.17f;
    
    int _lastCount = -1;

    Vector2 _targetPosition;
    Quaternion _targetRotation;

    public ShapeContainerObject(ShapeContainer container)
    {
        this.container = container;
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
                                  new Vector2(offsetX, offsetY).Rotate90(true,
                                      Utils.DirFromCoords(matrix.currentShapeDir));
        _targetRotation = Quaternion.AngleAxis(-90f * Utils.DirFromCoords(matrix.currentShapeDir), Vector3.forward);
        
        transform.localPosition =
            Vector2.Lerp(transform.localPosition, _targetPosition, Time.deltaTime * LerpMultiplier);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, _targetRotation, Time.deltaTime * LerpMultiplier);
    }

    void OnValidate()
    {
        if (container != null)
            RefreshShapesPlacement();
    }

    void RefreshShapesPlacement()
    {
        for (var i = container.currentIndex; i < container.shapes.Count; i++)
        {
            var shape = container.shapes[i];
            var t = i - container.currentIndex + 1;
            shape.shapeObject.SetTargetPosition(
                new Vector3(t * xMult / Mathf.Pow(xAccum, t), t * yMult / Mathf.Pow(yAccum, t)));
            shape.shapeObject.SetTargetScale(Vector3.one / t / scaleMult);
        }
    }
}