using System;
using UnityEngine;

public class ScreenBox : MonoBehaviour
{
    public static ScreenBox activeBox;
    protected virtual void OnEnable()
    {
        activeBox = this;
    }
    
    protected virtual void OnDisable()
    {
        activeBox = null;
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        if (activeBox != null) activeBox.Hide(); 
        gameObject.SetActive(true);
    }
}