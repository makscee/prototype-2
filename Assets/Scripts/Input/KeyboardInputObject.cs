using System;
using System.Linq;
using UnityEngine;

public class KeyboardInputObject : MonoBehaviour
{
    public static Action onAnyKey;
    
    [SerializeField] KeyCode[] left, right, up, down, enter;
    
    void Update()
    {
        if (GameManager.IsTrailer || GameManager.IsEnding) return;
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftControl))
        {
            leftDownTs = float.MaxValue;
            rightDownTs = float.MaxValue;
            return;
        }
#endif
        if (Input.anyKeyDown)
        {
            onAnyKey?.Invoke();
            onAnyKey = null;
        }
        
        CheckLeftRightKeys();
        CheckUpDownKeys();
        CheckEnterKeys();
    }

    float leftDownTs, rightDownTs;
    void CheckLeftRightKeys()
    {
        if (IsKeyDown(3))
        {
            InputSystem.LeftPress();
            leftDownTs = Time.time;
        }
        if (IsKeyDown(1))
        {
            InputSystem.RightPress();
            rightDownTs = Time.time;
        }

        var curTs = Time.time;
        var leftHeld = IsKeyPressed(3);
        var rightHeld = IsKeyPressed(1);
        if (leftHeld || rightHeld)
        {
            var sinceDown = curTs - (leftHeld ? leftDownTs : rightDownTs);
            var repeatDelay = GlobalConfig.Instance.keyRepeatDelay;
            if (sinceDown > repeatDelay)
            {
                var repeatPeriod = GlobalConfig.Instance.keyRepeatPeriod;
                var repeats = Mathf.Floor((sinceDown - repeatDelay) / repeatPeriod);
                var prevRepeats = Mathf.Floor((sinceDown - repeatDelay - Time.deltaTime) / repeatPeriod);
                if (prevRepeats < repeats)
                {
                    if (leftHeld)
                        InputSystem.LeftPress();
                    else InputSystem.RightPress();
                }
            }
        }
    }

    void CheckUpDownKeys()
    {
        if (IsKeyDown(0))
        {
            InputSystem.UpPress();
        }
        if (IsKeyDown(2))
        {
            InputSystem.DownPress();
        }
    }

    void CheckEnterKeys()
    {
        if (enter.Any(Input.GetKeyDown))
            InputSystem.EnterPress();
    }

    bool IsKeyDown(int dir)
    {
        switch (dir)
        {
            case 0:
                return up.Any(Input.GetKeyDown);
            case 1:
                return right.Any(Input.GetKeyDown);
            case 2:
                return down.Any(Input.GetKeyDown);
            case 3:
                return left.Any(Input.GetKeyDown);
        }
        return false;
    }

    bool IsKeyPressed(int dir)
    {
        
        switch (dir)
        {
            case 0:
                return up.Any(Input.GetKey);
            case 1:
                return right.Any(Input.GetKey);
            case 2:
                return down.Any(Input.GetKey);
            case 3:
                return left.Any(Input.GetKey);
        }
        return false;
    }
}