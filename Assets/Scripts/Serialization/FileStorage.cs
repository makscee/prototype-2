using System.IO;
using UnityEngine;

public static class FileStorage
{
    static readonly string PathBase = Application.persistentDataPath + "/";

    public static string ReadJsonFile(string filename)
    {
        var path = Path(filename);
        Debug.Log($"{path}");
        if (!File.Exists(path)) return "{}";
        var text = File.ReadAllText(path);
        Debug.Log($"Load file {filename}:\n{text}");
        return text;
    }
    
    public static void SaveJsonToFile(string json, string filename)
    {
        var path = Path(filename);
        File.WriteAllText(path, json);
        Debug.Log($"save json to {path}:\n{json}");
    }

    public static string LevelPath(string name, bool full)
    {
        return LevelsFolderPath(full) + $"/{name}.json";
    }

    public static string LevelsFolderPath(bool full)
    {
        return (full ? PathBase : "") + "levels";
    }

    public static string Path(string filename, string extension = "json")
    {
        if (!filename.EndsWith($".{extension}")) filename += $".{extension}";
        return $"{PathBase}{filename}";
    }

    public static string PathToFolder(string name)
    {
        return $"{PathBase}{name}";
    }
}