using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsStorage", menuName = "ScriptableObjects/LevelsStorage")]
public class LevelsStorage : ScriptableObject
{
    public static LevelsStorage Instance => GetInstance();
    static LevelsStorage _instanceCache;
    static LevelsStorage GetInstance()
    {
        if (_instanceCache == null)
            _instanceCache = Resources.Load<LevelsStorage>("LevelsStorage");
        return _instanceCache;
    }
    
    [SerializeField] List<LevelData> levels;
    Dictionary<string, string> levelsMap;

    public string GetLevel(int packId, int fieldId)
    {
        if (levelsMap == null)
            InitLevelsMap();
        var key = $"{packId}_{fieldId}";
        if (!levelsMap.ContainsKey(key))
            return levelsMap["0_0"];
        return levelsMap[key];
    }

    public void SaveLevel(int packId, int fieldId, string data)
    {
        var key = $"{packId}_{fieldId}";
        var ld = new LevelData {key = key, data = data};
        if (levelsMap == null) InitLevelsMap();
        if (levelsMap.ContainsKey(key))
        {
            levelsMap[key] = data;
            for (var i = 0; i < levels.Count; i++)
                if (levels[i].key == key)
                {
                    levels[i] = ld;
                    break;
                }
        }
        else
        {
            levelsMap.Add(key, data);
            levels.Add(ld);
        }
        
    }

    void InitLevelsMap()
    {
        levelsMap = new Dictionary<string, string>(levels.Count);
        foreach (var levelData in levels)
        {
            levelsMap.Add(levelData.key, levelData.data);
        }
    }
}

[Serializable]
public class LevelData
{
    public string key, data;
}