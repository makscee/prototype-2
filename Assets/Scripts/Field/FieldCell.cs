using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class FieldCell : MonoBehaviour
{
    const float AlphaDefault = 0.3f, AlphaProjectionShape = 0.6f, AlphaProjectionTrail = 0.40f, AlphaSelectScreen = 0.15f;
    
    public int X, Y;
    public FieldMatrix field;
    [SerializeField] SpriteRenderer sr;
    Color _originalColor;

    public Shape OccupiedBy { get; set; }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = GlobalConfig.Instance.palette1;
        _originalColor = sr.color;
    }

    void SetCoords(int x, int y)
    {
        X = x;
        Y = y;
        transform.localPosition = (Vector3)(field.ZeroPos + new Vector2(x, y)) + new Vector3(0f, 0f, 0.1f);
    }

    FieldCellState _state;
    public void SetState(FieldCellState state)
    {
        switch (state)
        {
            case FieldCellState.ShapeProjection:
                sr.color = _originalColor.ChangeAlpha(AlphaProjectionShape);
                break;
            case FieldCellState.ShapeProjectionTrail:
                sr.color = _originalColor.ChangeAlpha(AlphaProjectionTrail);
                break;
            case FieldCellState.ActiveEmpty:
                sr.color = _originalColor.ChangeAlpha(AlphaDefault);
                break;
            case FieldCellState.SelectScreen:
                sr.color = _originalColor.ChangeAlpha(AlphaSelectScreen);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        _state = state;
    }

    public void Destroy()
    {
        DestroyImmediate(gameObject);
    }

    public static FieldCell Create(FieldMatrix matrix, int x, int y, Transform parent)
    {
        var go = Instantiate(Prefabs.Instance.fieldCell, parent);
        var fc = go.GetComponent<FieldCell>();
        fc.field = matrix;
        fc.SetCoords(x, y);
        return fc;
    }
}