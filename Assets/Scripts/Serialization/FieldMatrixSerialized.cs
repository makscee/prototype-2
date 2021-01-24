using System;
using System.IO;
using UnityEngine;

[Serializable]
public class FieldMatrixSerialized : JsonUtilitySerializable
{
    public int size;
    public ShapeContainerSerialized container;

    public FieldMatrixSerialized(FieldMatrix field)
    {
        container = new ShapeContainerSerialized(field.shapesContainer);
        size = field.Size;
    }

    public FieldMatrixSerialized(string json)
    {
        var data = JsonUtility.FromJson<FieldMatrixSerialized>(json);
        size = data.size;
        container = data.container;
    }

    public FieldMatrix CreateField()
    {
        var field = FieldMatrix.Create();
        field.Size = size;
        // field.SetContainer(container.Deserialize(field));
        field.CreateCells();
        return field;
    }

    public void LoadShapesContainer(FieldMatrix field)
    {
        field.Size = size;
        field.SetContainer(container.Deserialize(field));
    }

    const string LevelsNameTemplate = "level_{0}_{1}"; // {pack_id}_{field_id}
    const string LevelsFolder = "levels";

    public void SaveToFile(int packId, int fieldId)
    {
        SaveToFile(GetFileName(packId, fieldId));
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
        var fileName = GetFileName(packId, fieldId);
        var json = FileStorage.ReadJsonFile($"{LevelsFolder}/{fileName}");
        if (json == null)
            json = FileStorage.ReadJsonFile($"{LevelsFolder}/level_placeholder");
        if (json == null) return null;
        var fieldSerialized = new FieldMatrixSerialized(json);
        return fieldSerialized;
    }

    public static bool FileExists(int packId, int fieldId)
    {
        var fileName = GetFileName(packId, fieldId);
        return File.Exists(FileStorage.ToFullPath($"{LevelsFolder}/{fileName}"));
    }
}