using System;
using UnityEngine;

public class InputCollider : MonoBehaviour
{
    [SerializeField] FieldMatrix field;
    void OnMouseEnter()
    {
        field.SetHovered(true);
    }

    void OnMouseExit()
    {
        field.SetHovered(false);
    }
}