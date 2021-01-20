using System;
using UnityEngine;

[ExecuteInEditMode]
public class FieldPack : MonoBehaviour
{
    public static FieldPack active;
    
    const float PushApartMult = 1.1f;
    [SerializeField] int sideSize = 3;
    int FieldsCount => sideSize == 3 ? 8 : 4;
    public int packId;
    public FieldMatrix[] fields;

    public float Height => sideSize * transform.localScale.x;
    void LoadFields()
    {
        fields = new FieldMatrix[FieldsCount];
        for (var fieldId = 0; fieldId < FieldsCount; fieldId++)
        {
                var field = FieldMatrixSerialized.Load(packId, fieldId)?.Deserialize();
                if (field == null) continue;
                field.transform.SetParent(transform);
                fields[fieldId] = field;
        }
    }
    
    void Clear()
    {
        fields = new FieldMatrix[FieldsCount];
        var childFields = GetComponentsInChildren<FieldMatrix>();
        foreach (var t in childFields) t.Destroy();
    }

    void PlaceFields()
    {
        for (var fieldId = 0; fieldId < FieldsCount; fieldId++)
        {
                var field = fields[fieldId];
                if (field == null) continue;
                var scale = 1f / field.Size;
                field.transform.localScale = new Vector3(scale, scale, scale);
                field.transform.localPosition = FieldIdToUnitPos(fieldId) * PushApartMult;
        }
    }

    public void SetFieldsState()
    {
        for (var fieldId = 0; fieldId < FieldsCount; fieldId++)
        {
            var field = fields[fieldId];
            field.SetState(FieldState.OnSelectScreen);
            field.SetCompletion(fieldId == 0 ? FieldCompletion.Unlocked : FieldCompletion.Locked);
        }
    }

    Vector2 FieldIdToUnitPos(int fieldId)
    {
        if (sideSize == 2)
        {
            switch (fieldId)
            {
                case 0:
                    return new Vector2(-.5f, .5f);
                case 1:
                    return new Vector2(.5f, .5f);
                case 2:
                    return new Vector2(-.5f, -.5f);
                case 3:
                    return new Vector2(.5f, -.5f);
            }
        } else if (sideSize == 3)
        {
            switch (fieldId)
            {
                case 0:
                    return new Vector2(-1f, 1f);
                case 1:
                    return new Vector2(0f, 1f);
                case 2:
                    return new Vector2(1f, 1f);
                case 3:
                    return new Vector2(-1f, 0f);
                case 4:
                    return new Vector2(1f, 0f);
                case 5:
                    return new Vector2(-1f, -1f);
                case 6:
                    return new Vector2(0f, -1f);
                case 7:
                    return new Vector2(1f, -1f);
            }
        }
        throw new Exception();
    } 

    public static FieldPack Create(int packId)
    {
        var go = new GameObject($"FieldPack{packId}");
        var size = packId == 0 ? 2 : 3;
        var fp = go.AddComponent<FieldPack>();
        fp.sideSize = size;
        fp.packId = packId;
        
        fp.LoadFields();
        fp.PlaceFields();
        return fp;
    }
}