using System.Collections.Generic;

public static class Progress
{
    public const int Version = 3;
    static HashSet<string> _data;
    static bool _trackingPaused;

    public static HashSet<string> Data
    {
        get
        {
            if (_data == null)
                Load();
            return _data;
        }
        set => _data = value;
    }

    public static bool IsComplete(int packId, int fieldId)
    {
        return Data.Contains($"{packId}_{fieldId}");
    }

    public static bool IsComplete(FieldMatrix field)
    {
        return IsComplete(field.packId, field.fieldId);
    }

    public static void SetComplete(int packId, int fieldId)
    {
        if (_trackingPaused) return;
        Data.Add($"{packId}_{fieldId}");
        Save();
    }

    public static void UnsetComplete(int packId, int fieldId)
    {
        var key = $"{packId}_{fieldId}";
        if (Data.Contains(key)) Data.Remove(key);
        Save();
    }

    public static void Load()
    {
        Data = new HashSet<string>(ProgressSerialized.Load().data);
    }

    public static void Save()
    {
        new ProgressSerialized(Data).SaveToFile();
    }

    public static void ResetAndSave()
    {
        Data.Clear();
        Save();
    }

    public static void PauseTracking(bool value)
    {
        _trackingPaused = value;
    }

    public static void ResetPackAndSave(int packId)
    {
        foreach (var field in FieldPacksCollection.Packs[packId].fields)
        {
            var key = $"{packId}_{field.fieldId}";
            if (Data.Contains(key))
                Data.Remove(key);
        }
        Save();
    }
}