using System.Collections.Generic;
using UnityEngine;

public class ShapeContainer
{
    public List<Shape> shapes = new List<Shape>();
    public Vector2Int matrixSize;
    public FieldMatrix matrix;
    ShapeContainerObject _containerObject;

    public ShapeContainer(FieldMatrix matrix)
    {
        this.matrix = matrix;
        _containerObject = new GameObject("Container Object").AddComponent<ShapeContainerObject>();
        _containerObject.container = this;
        _containerObject.transform.SetParent(matrix.transform);
        _containerObject.matrix = matrix;
    }

    public void Add(Shape shape, int ind = -1)
    {
        shape.shapeObject.transform.SetParent(_containerObject.transform);
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

    public void SaveToFile(string filename)
    {
        var serializedContainer = new ShapeContainerSerialized(this);
        var json = serializedContainer.ToJson();
        FileStorage.SaveJsonToFile(json, filename);
    }

    public void Destroy()
    {
        foreach (var shape in shapes) shape.Destroy();
        Object.Destroy(_containerObject.gameObject);
    }
}