using System;
using UnityEngine;

public class PfxCamera : MonoBehaviour
{
    public Camera mainCamera;
    public Camera selfCamera;

    void Update()
    {
        selfCamera.orthographicSize = mainCamera.orthographicSize;
        var selfT = transform;
        var mainCamT = mainCamera.transform;
        selfT.position = mainCamT.position;
        selfT.rotation = mainCamT.rotation;
    }
}