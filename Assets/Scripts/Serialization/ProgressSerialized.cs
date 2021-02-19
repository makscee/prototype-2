using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ProgressSerialized : JsonUtilitySerializable
{
    public List<string> data;
    const string Path = "progress";

    public ProgressSerialized(IEnumerable<string> data)
    {
        this.data = new List<string>(data);
    }
    
    public void SaveToFile()
    {
        FileStorage.SaveJsonToFile(ToJson(), Path);
    }

    public static ProgressSerialized Load()
    {
        var json = FileStorage.ReadJsonFile(Path);
        if (json == null) json = "{}";
        return JsonUtility.FromJson<ProgressSerialized>(json);
    }
}