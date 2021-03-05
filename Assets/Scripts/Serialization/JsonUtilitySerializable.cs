using UnityEngine;

public abstract class JsonUtilitySerializable
{    
    public string ToJson()
    {
        return JsonUtility.ToJson(this, false);
    }
}