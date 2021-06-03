using System;
using Unity.Mathematics;
using UnityEngine;

public class BackgroundPattern : MonoBehaviour
{
    [SerializeField] Camera cam;

    Vector3 _initialScale;
    void Awake()
    {
        _initialScale = transform.localScale;
    }

    void Update()
    {
        var v = _initialScale.x * Mathf.Sqrt(cam.orthographicSize / 3.5f);
        transform.localScale = new Vector3(v, v, 1);
    }
}