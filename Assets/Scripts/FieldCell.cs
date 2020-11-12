using UnityEngine;

public class FieldCell : MonoBehaviour
{
    public int X, Y;
    Shape occupiedBy;
    public FieldMatrix matrix;
    SpriteRenderer _sr;

    public Shape OccupiedBy
    {
        get => occupiedBy;
        set
        {
            Debug.Log($"update");
            occupiedBy = value;
            _sr.color = _sr.color.ChangeAlpha(value == null ? 0.3f : 0f);
        }
    }

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    void SetCoords(int x, int y)
    {
        X = x;
        Y = y;
        transform.localPosition = matrix.ZeroPos + new Vector2(x, y);
    }

    public void Destroy()
    {
        DestroyImmediate(gameObject);
    }

    public static FieldCell Create(FieldMatrix matrix, int x, int y)
    {
        var go = Instantiate(Prefabs.Instance.fieldCell, matrix.transform);
        var fc = go.GetComponent<FieldCell>();
        fc.matrix = matrix;
        fc.SetCoords(x, y);
        return fc;
    }
}