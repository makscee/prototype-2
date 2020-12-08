using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShapeContainerSerialized : JsonUtilitySerializable
{
    public List<ShapeSerialized> shapes;

    public ShapeContainerSerialized(ShapeContainer container)
    {
        shapes = new List<ShapeSerialized>(container.shapes.Count);

        var maxInd = container.currentIndex < container.shapes.Count
            ? container.currentIndex - 1
            : container.shapes.Count;
        for (var i = 0; i < maxInd; i++)
        {
            shapes.Add(new ShapeSerialized(container.shapes[i]));
        }
    }

    public ShapeContainer Deserialize(FieldMatrix matrix)
    {
        var container = new ShapeContainer(matrix);
        foreach (var shapeSerialized in shapes)
        {
            var shape = shapeSerialized.Deserialize();
            shape.Matrix = matrix;
            container.Add(shape);
        }
        return container;
    }

    public static ShapeContainerSerialized LoadFromJson(string json)
    {
        return JsonUtility.FromJson<ShapeContainerSerialized>(json);
    }
}