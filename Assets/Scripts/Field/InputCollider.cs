﻿using System;
using UnityEngine;

public class InputCollider : MonoBehaviour
{
    [SerializeField] FieldMatrix field;
    void OnMouseEnter()
    {
        if (field.completion == FieldCompletion.Unlocked)
            field.SetHovered(true);
    }

    void OnMouseExit()
    {
        // field.SetHovered(false);
    }
}