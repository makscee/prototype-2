using System;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class FieldPack : MonoBehaviour
{
    public static FieldPack active;
    
    const float PushApartMult = 1.5f;
    [SerializeField] int sideSize = 3;
    int FieldsCount => sideSize == 3 ? 8 : 4;
    public int packId;
    public FieldMatrix[] fields;

    [SerializeField] bool initFields;

    void OnValidate()
    {
        if (initFields)
        {
            initFields = false;
            LoadFields();
            PlaceFields();
        }
    }

    public float Height => sideSize * transform.localScale.x;

    void LoadFields()
    {
        fields = GetComponentsInChildren<FieldMatrix>();
        for (var fieldId = 0; fieldId < fields.Length; fieldId++)
        {
            var field = fields[fieldId];
            field.transform.SetParent(transform);
            field.fieldId = fieldId;
            field.packId = packId;
            field.gameObject.name = $"fm {packId}_{fieldId}";
        }
    }

    public bool Complete { get; private set; }
    public void FieldCompleted()
    {
        PlaceFields();
        if (fields.All(field => field.completion == FieldCompletion.Complete))
        {
            Complete = true;
            // active = FieldPacksCollection.Packs[packId + 1];
        }
    }

    void PlaceFields()
    {
        for (var fieldId = 0; fieldId < FieldsCount; fieldId++)
        {
            var field = fields[fieldId];
            PlaceField(field);
        }
    }

    public void PlaceField(FieldMatrix field)
    {
        var scale = 1f / field.Size;
        field.transform.localScale = new Vector3(scale, scale, scale);
        field.transform.localPosition = FieldIdToUnitPos(field.fieldId) * (field.completion == FieldCompletion.Complete ? 1f : PushApartMult);
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