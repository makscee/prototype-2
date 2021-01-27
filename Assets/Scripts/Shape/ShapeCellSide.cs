using System;
using UnityEngine;

[ExecuteInEditMode]
public class ShapeCellSide : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] int dir;

    FieldMatrix _field;

    void Start()
    {
        _field = GetComponentInParent<FieldMatrix>();
        Refresh();
    }

    void Update()
    {
        Refresh();
    }

    public void Enable(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetColor(Color c)
    {
        sr.color = c;
    }
    float Thickness => GlobalConfig.Instance.thicknessBase + _field.shapeSidesThickness;

    void Refresh()
    {
        var thickness = Mathf.Clamp(Thickness, -1f, 1f);
        var dirVec = Utils.CoordsFromDir(dir);
        float t;
        transform.localPosition = dirVec * new Vector2(t = thickness / 4, t);
        transform.localScale = new Vector3(t = 1f - thickness, t) + (Vector3)dirVec.Abs() * thickness / 2;
    }
}