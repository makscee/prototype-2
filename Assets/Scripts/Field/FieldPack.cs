using System;
using UnityEngine;

[ExecuteInEditMode]
public class FieldPack : MonoBehaviour
{
    public static FieldPack active;
    
    const float Padding = 0.1f;
    [SerializeField] int size = 3;
    public int packId;
    FieldMatrix[,] _fields;

    int _fieldsSize = 3;
    public float height => (size * _fieldsSize + (size - 1) * Padding) * transform.localScale.x;
    void LoadFields()
    {
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                if (i == 1 && j == 1 && size == 3) continue;
                var field = FieldMatrixSerialized.Load(packId, i, j)?.Deserialize();
                if (field == null) continue;
                field.transform.SetParent(transform);
                field.SetState(FieldState.OnSelectScreen);
                _fields[i, j] = field;
                _fieldsSize = field.Size;
            }
        }
    }
    
    void Clear()
    {
        _fields = new FieldMatrix[size, size];
        var childFields = GetComponentsInChildren<FieldMatrix>();
        for (var i = 0; i < childFields.Length; i++)
        {
            childFields[i].Destroy();
        }
    }

    void PlaceFields()
    {
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                var field = _fields[i, j];
                if (field == null) continue;
                field.transform.localPosition = new Vector3(j - (size - 1f) / 2, (size - 1 - i) - (size - 1f) / 2) *
                                                new Vector2(field.Size + Padding, field.Size + Padding);
            }
        }
    }

    public static FieldPack Create(int packId)
    {
        var go = new GameObject($"FieldPack{packId}");
        var size = packId == 0 ? 2 : 3;
        var fp = go.AddComponent<FieldPack>();
        fp.size = size;
        fp.packId = packId;
        
        fp.Clear();
        fp.LoadFields();
        fp.PlaceFields();
        return fp;
    }
}