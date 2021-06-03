using System;
using System.IO;
using UnityEngine;

[Serializable]
public class FieldMatrixSerialized : JsonUtilitySerializable
{
    public int size;
    public ShapeContainerSerialized container;

    public FieldMatrixSerialized(FieldMatrix field, int insertedShapesCount)
    {
        container = new ShapeContainerSerialized(field.shapesContainer, insertedShapesCount);
        size = field.Size;
    }

    public FieldMatrixSerialized(string json)
    {
        var data = JsonUtility.FromJson<FieldMatrixSerialized>(json);
        size = data.size;
        container = data.container;
    }

    public void LoadShapesContainer(FieldMatrix field)
    {
        field.Size = size;
        field.SetContainer(container.Deserialize(field));
    }

    public void SaveToFile(int packId, int fieldId)
    {
        var data = ToJson();
        Debug.Log($"Saving level {packId}_{fieldId}\n{data}");
        LevelsStorage.Instance.SaveLevel(packId, fieldId, data);
    }
    
    public static FieldMatrixSerialized Load(int packId, int fieldId)
    {
        var json = LevelsStorage.Instance.GetLevel(packId, fieldId);
        if (json == null) return null;
        var fieldSerialized = new FieldMatrixSerialized(json);
        return fieldSerialized;
    }
}