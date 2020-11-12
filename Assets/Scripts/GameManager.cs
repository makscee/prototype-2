using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            var shape = Shape.Create(ShapeStrings.L);
            shape.UpDirection = Vector2Int.left;
            var matrix = FindObjectOfType<FieldMatrix>();
            matrix.AttachShape(shape);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            var matrix = FindObjectOfType<FieldMatrix>();
            matrix.MoveAttachedShape(false);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            var matrix = FindObjectOfType<FieldMatrix>();
            matrix.MoveAttachedShape(true);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            var matrix = FindObjectOfType<FieldMatrix>();
            matrix.attachedShape.RotateClockwise();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            var matrix = FindObjectOfType<FieldMatrix>();
            matrix.InsertShape();
        }
    }
}