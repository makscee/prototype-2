using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShapeContainerSerialized : JsonUtilitySerializable
{
    public List<ShapeSerialized> shapes;

    public ShapeContainerSerialized(ShapeContainer container, int insertedShapesCount)
    {
        shapes = new List<ShapeSerialized>(container.shapes.Count);
        for (var i = 0; i < insertedShapesCount; i++)
            shapes.Add(new ShapeSerialized(container.shapes[i]));
    }

    public ShapeContainer Deserialize(FieldMatrix matrix)
    {
        var container = new ShapeContainer(matrix);
        foreach (var shapeSerialized in shapes)
        {
            var shape = shapeSerialized.Deserialize();
            shape.Field = matrix;
            container.Add(shape);
        }
        return container;
    }
}