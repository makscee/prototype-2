using System;
using UnityEngine;

public class FieldCell : MonoBehaviour
{
    const float AlphaDefault = 0.3f, AlphaProjectionShape = 0.6f, AlphaProjectionTrail = 0.40f, AlphaSelectScreen = 0.15f;

    public FieldMatrix field;
    public int X, Y;
    Vector2 _posOffset;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Color originalColor;

    public Shape OccupiedBy { get; set; }

    public Vector2 PosOffset
    {
        get => _posOffset;
        set
        {
            _posOffset = value;
            RefreshPosition();
        }
    }

    void SetCoords(int x, int y)
    {
        X = x;
        Y = y;
        RefreshPosition();
    }

    void RefreshPosition()
    {
        transform.localPosition =
            (Vector3) (field.ZeroPos + new Vector2(X, Y) + _posOffset) + new Vector3(0f, 0f, 0.1f);
    }

    FieldCellState _state;
    public void SetState(FieldCellState state)
    {
        switch (state)
        {
            case FieldCellState.ShapeProjection:
                sr.color = originalColor.ChangeAlpha(AlphaProjectionShape);
                break;
            case FieldCellState.ShapeProjectionTrail:
                sr.color = originalColor.ChangeAlpha(AlphaProjectionTrail);
                break;
            case FieldCellState.ActiveEmpty:
                sr.color = originalColor.ChangeAlpha(AlphaDefault);
                break;
            case FieldCellState.SelectScreen:
                sr.color = originalColor.ChangeAlpha(AlphaSelectScreen);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        _state = state;
    }

    public void FieldCompletionStateChangeHandle(FieldCompletion value)
    {
        var t = transform;
        switch (value)
        {
            case FieldCompletion.Locked:
                PosOffset = transform.position.normalized * 3f;
                t.localScale = new Vector3(0.7f, 0.7f);
                break;
            case FieldCompletion.Unlocked:
                var startPosOffset = PosOffset;
                var startScale = t.localScale;
                Animator.Interpolate(0f, 1f, GlobalConfig.Instance.fieldCellsAnimationTime)
                    .PassValue(v =>
                    {
                        PosOffset = Vector3.Lerp(startPosOffset, Vector3.zero, v);
                        t.localScale = Vector3.Lerp(startScale, Vector3.one, v);
                    });
                break;
            case FieldCompletion.Complete:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
    }
    

    public void Destroy()
    {
        DestroyImmediate(gameObject);
    }

    public static FieldCell Create(FieldMatrix field, int x, int y, Transform parent)
    {
        var go = Instantiate(Prefabs.Instance.fieldCell, parent);
        var fc = go.GetComponent<FieldCell>();
        fc.sr = fc.GetComponent<SpriteRenderer>();
        fc.sr.color = GlobalConfig.Instance.palette1;
        fc.originalColor = fc.sr.color;
        fc.field = field;
        fc.SetCoords(x, y);
        return fc;
    }
}