using System;
using UnityEngine;

// [ExecuteInEditMode]
public class ShapeCellSide : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] int dir;

    public void Enable(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetColor(Color c)
    {
        sr.color = c;
    }

    public void Refresh(float thickness)
    {
        thickness = Mathf.Clamp(thickness, -1f, 1f);
        var dirVec = Utils.CoordsFromDir(dir);
        float t;
        transform.localPosition = dirVec * new Vector2(t = thickness / 4, t);
        transform.localScale = new Vector3(t = 1f - thickness, t) + (Vector3)dirVec.Abs() * thickness / 2;
    }
}