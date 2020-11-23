using UnityEngine;

public class FieldCell : MonoBehaviour
{
    public int X, Y;
    Shape occupiedBy;
    public FieldMatrix matrix;
    const float AlphaDefault = 0.3f, AlphaProjection = 0.6f;
    SpriteRenderer _sr;
    Color original;

    [SerializeField] bool debug;
    public Shape OccupiedBy
    {
        get => occupiedBy;
        set
        {
            occupiedBy = value;
            // if (debug) _sr.color = _sr.color.ChangeAlpha(value == null ? AlphaDefault : 0f);
        }
    }

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        original = _sr.color;
    }

    void SetCoords(int x, int y)
    {
        X = x;
        Y = y;
        transform.localPosition = matrix.ZeroPos + new Vector2(x, y);
    }

    public void SetProjection(int value)
    {
        switch (value)
        {
            case 0:
                _sr.color = original.ChangeAlpha(AlphaDefault);
                break;
            case 1:
                _sr.color = original.ChangeAlpha(AlphaProjection);
                break;
            case 2:
                _sr.color = matrix.attachedShape.color.ChangeAlpha(AlphaProjection);
                break;
        }
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