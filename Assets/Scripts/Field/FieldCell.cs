using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FieldCell : MonoBehaviour
{
    const float ActiveScale = 0.95f, LerpDefault = 0.2f, LerpProjectionTrail = 0.5f, LerpProjectionShape = 1f;

    public FieldMatrix field;
    public int X, Y;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] float sinOffset;
    [SerializeField] float colorLerp, colorAlpha;

    Vector3Target _posOffset = new Vector3Target(Vector3.zero);
    FloatTarget _scale = new FloatTarget(1f);
    [SerializeField] Vector3 lockedOffset;
    [SerializeField] float targetProgressSpeed = 1f;

    public Shape OccupiedBy { get; set; }

    void Start()
    {
        field.onShapePlaced += OnShapePlaced;
        PostFxController.Instance.SubscribeToColors(SetColors);
    }

    void OnDestroy()
    {
        if (PostFxController.Instance != null)
            PostFxController.Instance.UnsubscribeFromColors(SetColors);
    }

    IReadOnlyList<Color> _colors;
    void SetColors(IReadOnlyList<Color> colors)
    {
        _colors = colors;
        SetColorLerp(colorLerp);
    }

    void OnShapePlaced()
    {
        _scale.value = .3f;
    }

    public void RefreshTargets()
    {
        if (field.screenState == FieldScreenState.OnSelectScreen)
        {
            switch (field.completion)
            {
                case FieldCompletion.Locked:
                    _posOffset.target = lockedOffset;
                    _scale.target = 0.5f;
                    SetAlpha(0.5f);
                    sr.sortingOrder = 0;
                    break;
                case FieldCompletion.Unlocked:
                    _posOffset.target = Vector3.zero;
                    _scale.target = 1f;
                    SetAlpha(0.8f);
                    sr.sortingOrder = 2;
                    break;
                case FieldCompletion.Complete:
                    _posOffset.target = Vector3.zero;
                    _scale.target = 1f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        } else if (field.screenState == FieldScreenState.Active)
        {
            _scale.target = ActiveScale;
        }
    }

    void SetCoords(int x, int y)
    {
        X = x;
        Y = y;
        sinOffset = X + Y;
        targetProgressSpeed = Mathf.Lerp(8f, 3f, (X + Y) / (field.Size - 1f) / 2f);
        lockedOffset = Random.onUnitSphere * 9f;
        RefreshTransform();
    }

    void RefreshTransform()
    {
        transform.localPosition =
            (Vector3) (field.ZeroPos + new Vector2(X, Y)) + _posOffset.value;// + new Vector3(0f, 0f, 0.1f);
        transform.localScale = new Vector3(_scale.value, _scale.value, _scale.value);
    }

    void Update()
    {
        if (field.screenState == FieldScreenState.OnSelectScreen && field.completion == FieldCompletion.Unlocked)
        {
            SetColorLerp(LerpDefault + Mathf.Sin(Time.time + sinOffset) / 10f);
        }

        var delta = Time.deltaTime * targetProgressSpeed;
        _scale.ProgressToTarget(delta);
        _posOffset.ProgressToTarget(delta);
        RefreshTransform();
    }
    public void SetProjectionState(FieldProjectionState state)
    {
        SetAlpha(0.8f);
        switch (state)
        {
            case FieldProjectionState.ShapeProjection:
                SetColorLerp(LerpProjectionShape);
                break;
            case FieldProjectionState.ShapeProjectionTrail:
                SetColorLerp(LerpProjectionTrail);
                break;
            case FieldProjectionState.Empty:
                SetColorLerp(LerpDefault);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    void SetColorLerp(float value)
    {
        sr.color = Color.Lerp(_colors[0], _colors[1], value)
            .ChangeAlpha(colorAlpha);
        colorLerp = value;
    }

    void SetAlpha(float value)
    {
        sr.color = sr.color.ChangeAlpha(value);
        colorAlpha = value;
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
        fc.field = field;
        fc.SetCoords(x, y);
        return fc;
    }
}