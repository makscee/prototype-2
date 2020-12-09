using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShapeContainerSerialized : JsonUtilitySerializable
{
    public List<ShapeSerialized> shapes;
    public int sizeX, sizeY;

    public ShapeContainerSerialized(ShapeContainer container)
    {
        sizeX = container.matrix.size.x;
        sizeY = container.matrix.size.y;
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
        container.matrixSize = new Vector2Int(sizeX, sizeY);
        return container;
    }

    public static ShapeContainerSerialized LoadFromJson(string json)
    {
        return JsonUtility.FromJson<ShapeContainerSerialized>(json);
    }

    const string LastLevelPrefsKey = "last_loaded_level";
    public static ShapeContainerSerialized LoadByName(string name)
    {
        var container = LoadFromJson(FileStorage.ReadJsonFile(FileStorage.LevelPath(name, false)));
        if (container != null) PlayerPrefs.SetString(LastLevelPrefsKey, name);
        return container;
    }

    public static ShapeContainerSerialized GetLastLoadedLevel()
    {
        if (!PlayerPrefs.HasKey(LastLevelPrefsKey)) return null;
        var name = PlayerPrefs.GetString(LastLevelPrefsKey);
        return LoadByName(name);
    }
}