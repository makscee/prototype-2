using System.IO;
using UnityEngine;

public static class FileStorage
{
    static readonly string PathBase = Application.persistentDataPath + "/";

    public static string ReadJsonFile(string filePath)
    {
        var path = ToFullPath(filePath);
        // Debug.Log($"{path}");
        if (!File.Exists(path))
        {
            // Debug.Log($"File not found");
            return null;
        }

        var text = File.ReadAllText(path);
        // Debug.Log($"Load file {filePath}:\n{text}");
        return text;
    }

    public static string ReadJsonFileFromResources(string filePath)
    {
        var asset = Resources.Load<TextAsset>(filePath);
        if (asset == null) return null;
        return asset.text;
    }
    
    public static void SaveJsonToFile(string json, string filename)
    {
        var path = ToFullPath(filename);
        File.WriteAllText(path, json);
        Debug.Log($"save json to {path}:\n{json}");
    }

    public static string ToFullPath(string filename, string extension = "json")
    {
        if (!filename.EndsWith($".{extension}")) filename += $".{extension}";
        return $"{PathBase}{filename}";
    }
}