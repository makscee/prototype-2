using System.IO;
using UnityEngine;

public static class FileStorage
{
    static readonly string PathBase = Application.persistentDataPath + "/";

    public static string ReadJsonFile(string filename)
    {
        if (!File.Exists(Path(filename))) return "{}";
        return File.ReadAllText(Path(filename));
    }
    
    public static void SaveJsonToFile(string json, string filename)
    {
        var path = Path(filename);
        File.WriteAllText(path, json);
        Debug.Log($"save json to {path}\n{json}");
    }

    static string Path(string fileName, string extension = "json")
    {
        return $"{PathBase}{fileName}.{extension}";
    }
}