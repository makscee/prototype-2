using System;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class LevelSelectorBox : ScreenBox
{
    [SerializeField] Transform entryParent;
    [SerializeField] bool needRefresh;
    
    string[] GetAllLevelNames()
    {
        var d = new DirectoryInfo(FileStorage.LevelsFolderPath(true));
        const string extension = ".json";
        var files = d.GetFiles($"*{extension}");
        var result = new string[files.Length];
        for (var i = 0; i < files.Length; i++)
        {
            var filename = files[i].Name;
            result[i] = filename.Substring(0, filename.Length - extension.Length);
        }

        return result;
    }

    void OnValidate()
    {
        needRefresh = false;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += Refresh;
#endif
    }

    protected void Refresh()
    {
        foreach (var entry in entryParent.GetComponentsInChildren<LevelSelectorEntry>())
            DestroyImmediate(entry.gameObject);
        foreach (var filename in GetAllLevelNames())
        {
            var entry = LevelSelectorEntry.Create(entryParent, filename);
            entry.onClick += OnEntryClick;
        }
    }

    protected virtual void OnEntryClick(LevelSelectorEntry entry)
    {
        
    }
}