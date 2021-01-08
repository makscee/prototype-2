using System;
using UnityEngine;

[ExecuteInEditMode]
public class ShapeCellInside : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] int dir;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        Refresh();
    }

    void OnValidate()
    {
        // Refresh();
    }

    float _thicknessBefore = -1f;
    void Update()
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (_thicknessBefore != GlobalConfig.Instance.thickness)
        {
            _thicknessBefore = GlobalConfig.Instance.thickness;
            Refresh();
        }
    }

    public void Enable(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetColor(Color c)
    {
        sr.color = c;
    }

    void Refresh()
    {
        var thickness = Mathf.Clamp(GlobalConfig.Instance.thickness, -1f, 1f);
        var dirVec = Utils.CoordsFromDir(dir);
        float t;
        transform.localPosition = dirVec * new Vector2(t = thickness / 4, t);
        transform.localScale = new Vector3(t = 1f - thickness, t) + (Vector3)dirVec.Abs() * thickness / 2;
    }
}