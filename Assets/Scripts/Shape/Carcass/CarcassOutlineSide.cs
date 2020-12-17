using System;
using UnityEngine;

[ExecuteInEditMode]
public class CarcassOutlineSide : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] int dir;
    [SerializeField] bool isCorner;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public bool IsCorner
    {
        get => isCorner;
        set
        {
            isCorner = value;
            Refresh();
        }
    }

    void OnValidate()
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

    void Refresh()
    {
        var thickness = GlobalConfig.Instance.outlineThickness;
        if (IsCorner)
        {
            transform.localScale = new Vector3(thickness, thickness);
            var tPos = 0.5f - thickness / 2f;
            switch (dir)
            {
                case 0:
                    transform.localPosition = new Vector3(-tPos, tPos);
                    break;
                case 1:
                    transform.localPosition = new Vector3(tPos, tPos);
                    break;
                case 2:
                    transform.localPosition = new Vector3(tPos, -tPos);
                    break;
                case 3:
                    transform.localPosition = new Vector3(-tPos, -tPos);
                    break;
            }
        }
        else
        {
            switch (dir)
            {
                case 0:
                    transform.localScale = new Vector3(1f, thickness);
                    transform.localPosition = new Vector3(0f, 0.5f - thickness / 2f);
                    break;
                case 1:
                    transform.localScale = new Vector3(thickness, 1f);
                    transform.localPosition = new Vector3(0.5f - thickness / 2f, 0f);
                    break;
                case 2:
                    transform.localScale = new Vector3(1f, thickness);
                    transform.localPosition = new Vector3(0f, -0.5f + thickness / 2f);
                    break;
                case 3:
                    transform.localScale = new Vector3(thickness, 1f);
                    transform.localPosition = new Vector3(-0.5f + thickness / 2f, 0f);
                    break;
            }
        }
    }
}