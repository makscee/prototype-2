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
        foreach (var shape in container.shapes)
        {
            if (shape == null) Debug.Log($"found");
            shapes.Add(new ShapeSerialized(shape));
        }
    }

    public ShapeContainer Deserialize(FieldMatrix matrix)
    {
        var container = new ShapeContainer(matrix);
        foreach (var shapeSerialized in shapes) container.Add(shapeSerialized.Deserialize());
        return container;
    }

    public static ShapeContainerSerialized LoadFromJson(string json)
    {
        return JsonUtility.FromJson<ShapeContainerSerialized>(json);
    }
}