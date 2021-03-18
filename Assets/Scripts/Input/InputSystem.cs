using System;

public static class InputSystem
{
    public static Action onLeftPress, onRightPress, onUpPress, onDownPress;
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
}