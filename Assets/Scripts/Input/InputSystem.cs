using System;

public static class InputSystem
{
    public static Action onLeftPress, onRightPress, onUpPress, onDownPress, onEnterPress;
    public static void LeftPress()
    {
        onLeftPress?.Invoke();
    }

    public static void RightPress()
    {
        onRightPress?.Invoke();
    }

    public static void UpPress()
    {
        onUpPress?.Invoke();
    }

    public static void DownPress()
    {
        onDownPress?.Invoke();
    }

    public static void EnterPress()
    {
        onEnterPress?.Invoke();
    }
}