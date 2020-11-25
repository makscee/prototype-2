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
        File.WriteAllText(Path(filename), json);
    }

    static string Path(string fileName, string extension = "json")
    {
        return $"{PathBase}{fileName}.{extension}";
    }
}