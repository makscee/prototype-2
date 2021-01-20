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

    const string LevelsNameTemplate = "level_{0}_{1}"; // {pack_id}_{field_id}
    const string LevelsFolder = "levels";

    public void SaveToFile(int packID, int fieldID)
    {
        SaveToFile(GetFileName(packID, fieldID));
    }

    public void SaveToFile(string name)
    {
        FileStorage.SaveJsonToFile(ToJson(), $"{LevelsFolder}/{name}");
    }

    static string GetFileName(int packId, int fieldId)
    {
        return string.Format(LevelsNameTemplate, packId, fieldId);
    }
    
    public static FieldMatrixSerialized Load(int packId, int fieldId)
    {
        return Load(GetFileName(packId, fieldId), packId);
    }
    
    public static FieldMatrixSerialized Load(string file, int packId = -1)
    {
        // var json = FileStorage.ReadJsonFile($"{LevelsFolder}/{file}");
        var json = FileStorage.ReadJsonFile($"{LevelsFolder}/level_0_0");
        if (json == null) return null;
        var fieldSerialized = new FieldMatrixSerialized(json, packId);
        return fieldSerialized;
    }
}