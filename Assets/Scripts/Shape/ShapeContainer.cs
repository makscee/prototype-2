using System.Collections.Generic;
using UnityEngine;

public class ShapeContainer
{
    public readonly List<Shape> shapes = new List<Shape>();
    public readonly FieldMatrix matrix;
    readonly ShapeContainerObject _containerObject;

    public ShapeContainer(FieldMatrix matrix)
    {
        this.matrix = matrix;
        _containerObject = new GameObject("Shapes Container").AddComponent<ShapeContainerObject>();
        _containerObject.container = this;
        Transform transform;
        (transform = _containerObject.transform).SetParent(matrix.transform);
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;
        _containerObject.matrix = matrix;
    }

    public void Add(Shape shape, int ind = -1)
    {
        shape.shapeObject.SetParent(_containerObject.transform);
        shape.shapeObject.transform.localScale = shape.shapeObject.CurrentScaleTarget;
        shape.SetRotation(Vector2Int.up);
        ind = ind == -1 ? shapes.Count : ind;
        shapes.Insert(ind, shape);
    }

    public void InsertAtCurrent(Shape shape)
    {
        Add(shape, currentIndex);
    }

    public int currentIndex;
    public Shape GetNext()
    {
        if (currentIndex == shapes.Count) return null;
        return shapes[currentIndex++];
    }

    public void ReturnPrevious()
    {
        if (currentIndex == 0) return;
        currentIndex--;
        var shape = shapes[currentIndex]; 
        shape.shapeObject.SetParent(_containerObject.transform);
        shape.SetRotation(Vector2Int.up);
        shape.shapeObject.transform.localRotation = Quaternion.identity;
    }

    public void Destroy()
    {
        foreach (var shape in shapes) shape.Destroy();
        Object.Destroy(_containerObject.gameObject);
    }

    public void SetEnabled(bool value)
    {
        _containerObject.gameObject.SetActive(value);
        foreach (var shape in shapes) 
            shape.shapeObject.gameObject.SetActive(value);
    }
}