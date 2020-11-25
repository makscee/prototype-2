using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeContainer : IEnumerable<Shape>
{
    List<Shape> shapes = new List<Shape>();
    FieldMatrix _matrix;
    ShapeContainerObject _containerObject;

    public ShapeContainer(FieldMatrix matrix)
    {
        _matrix = matrix;
        _containerObject = new GameObject("Container Object").AddComponent<ShapeContainerObject>();
        _containerObject.shapes = shapes;
        _containerObject.transform.SetParent(matrix.transform);
        _containerObject.matrix = matrix;
    }

    public void Add(Shape shape)
    {
        shape.shapeObject.transform.SetParent(_containerObject.transform);
        shapes.Add(shape);
    }

    public Shape Pop()
    {
        if (shapes.Count == 0) return null;
        var shape = shapes[0];
        shapes.RemoveAt(0);
        return shape;
    }

    public IEnumerator<Shape> GetEnumerator()
    {
        return shapes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}