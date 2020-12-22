using UnityEngine;

[ExecuteInEditMode]
public class FieldPack : MonoBehaviour
{
    [SerializeField] int size = 3;
    public int packId;
    FieldMatrix[,] _fields;

    void LoadFields()
    {
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                _fields[i, j] = FieldMatrixSerialized.Load(packId, i, j)?.Deserialize();
            }
        }
    }
    
    void CollectChildren()
    {
        _fields = new FieldMatrix[size, size];
        var childFields = GetComponentsInChildren<FieldMatrix>();
        var ind = 0;
        for (var i = 0; i < size && ind < childFields.Length; i++)
        {
            for (var j = 0; j < size && ind < childFields.Length; j++)
            {
                _fields[i, j] = childFields[ind++];
            }
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
                field.transform.localPosition = new Vector3(size - 1 - i, j) * new Vector2(field.size, field.size);
            }
        }
    }
}