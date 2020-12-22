using System;
using UnityEngine;

[Serializable]
public class FieldMatrixSerialized : JsonUtilitySerializable
{
    public ShapeContainerSerialized container;

    public FieldMatrixSerialized(FieldMatrix field)
    {
        container = new ShapeContainerSerialized(field.shapesContainer);
    }

    public FieldMatrixSerialized(string json)
    {
        container = JsonUtility.FromJson<FieldMatrixSerialized>(json).container;
    }

    public FieldMatrix Deserialize()
    {
        var field = FieldMatrix.Create();
        field.SetContainer(container.Deserialize(field));
        return field;
    }

    const string LevelsNameTemplate = "level_{0}_{1}_{2}"; // {pack_id}_{field_row}_{field_column}

    public void SaveToFile(int packID, int rowID, int columnID)
    {
        FileStorage.SaveJsonToFile(ToJson(), GetFileName(packID, rowID, columnID));
    }

    static string GetFileName(int packID, int rowID, int columnID)
    {
        return string.Format(LevelsNameTemplate, packID, rowID, columnID);
    }
    
    public static FieldMatrixSerialized Load(int packID, int rowID, int columnID)
    {
        return Load(GetFileName(packID, rowID, columnID));
    }
    
    public static FieldMatrixSerialized Load(string file)
    {
        var json = FileStorage.ReadJsonFile(file);
        if (json == null) return null;
        var fieldSerialized = new FieldMatrixSerialized(json);
        return fieldSerialized;
    }
}