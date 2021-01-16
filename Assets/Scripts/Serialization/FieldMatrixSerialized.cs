using System;
using UnityEngine;

[Serializable]
public class FieldMatrixSerialized : JsonUtilitySerializable
{
    public ShapeContainerSerialized container;
    public int packId;

    public FieldMatrixSerialized(FieldMatrix field)
    {
        container = new ShapeContainerSerialized(field.shapesContainer);
    }

    public FieldMatrixSerialized(string json, int packId)
    {
        container = JsonUtility.FromJson<FieldMatrixSerialized>(json).container;
        this.packId = packId;
    }

    public FieldMatrix Deserialize()
    {
        var field = FieldMatrix.Create();
        field.packId = packId;
        field.SetContainer(container.Deserialize(field));
        return field;
    }

    const string LevelsNameTemplate = "level_{0}_{1}_{2}"; // {pack_id}_{field_row}_{field_column}
    const string LevelsFolder = "levels";

    public void SaveToFile(int packID, int rowID, int columnID)
    {
        SaveToFile(GetFileName(packID, rowID, columnID));
    }

    public void SaveToFile(string name)
    {
        FileStorage.SaveJsonToFile(ToJson(), $"{LevelsFolder}/{name}");
    }

    static string GetFileName(int packID, int rowID, int columnID)
    {
        return string.Format(LevelsNameTemplate, packID, rowID, columnID);
    }
    
    public static FieldMatrixSerialized Load(int packID, int rowID, int columnID)
    {
        return Load(GetFileName(packID, rowID, columnID), packID);
    }
    
    public static FieldMatrixSerialized Load(string file, int packId = -1)
    {
        // var json = FileStorage.ReadJsonFile($"{LevelsFolder}/{file}");
        var json = FileStorage.ReadJsonFile($"{LevelsFolder}/level_0_0_0");
        if (json == null) return null;
        var fieldSerialized = new FieldMatrixSerialized(json, packId);
        return fieldSerialized;
    }
}