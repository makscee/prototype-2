using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public enum InterpolationType
{
    Square, InvSquare, Linear, OverflowReturn
}

[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
public class Interpolator<T> : OwnedUpdatable
{
    float _over, _t, _delay;
    T _from, _to, _curValue, _curValueDelta;
    bool _active;
    Func<T, T, float, T> _interpolateFunc;
    Func<T, T, T> _subtractFunc;
    public Interpolator(T from, T to, float over, Func<T, T, float, T> interpolateFunc, Func<T, T, T> subtractFunc)
    {
        _from = from;
        _curValue = from;
        _to = to;
        _over = over;
        _interpolateFunc = interpolateFunc;
        _subtractFunc = subtractFunc;
    }

    public Interpolator<T> Play()
    {
        _isUpdating = true;
        return this;
    }

    public Interpolator<T> Stop()
    {
        _isUpdating = false;
        return this;
    }
    
    Action<T> _passDelta;
    public Interpolator<T> PassDelta(Action<T> action)
    {
        _passDelta = action;
        return this;
    }

    Action<T> _passValue;
    public Interpolator<T> PassValue(Action<T> action)
    {
        _passValue = action;
        return this;
    }

    public Action whenDone;
    public Interpolator<T> WhenDone(Action action)
    {
        whenDone = action;
        return this;
    }

    public Action whenStart;
    public Interpolator<T> WhenStart(Action action)
    {
        whenStart = action;
        return this;
    }

    Action _onDeltaSignChange;
    float _lastDeltaF, _lastF;
    public Interpolator<T> OnDeltaSignChange(Action action)
    {
        _onDeltaSignChange = action;
        return this;
    }

    float _allowedDeltaFrom = 0f, _allowedDeltaTo = 1f;
    public Interpolator<T> AllowedDelta(float from, float to) // [0f, 1f]
    {
        _allowedDeltaFrom = from;
        _allowedDeltaTo = to;
        return this;
    }
    public Interpolator<T> Delay(float t)
    {
        _delay = t;
        return this;
    }

    GameObject _nullCheck;
    bool _haveToNullCheck;

    public Interpolator<T> NullCheck(GameObject gameObject)
    {
        _haveToNullCheck = true;
        _nullCheck = gameObject;
        return this;
    }

    InterpolationType _interpolationType = InterpolationType.Linear;
    public Interpolator<T> Type(InterpolationType type)
    {
        _interpolationType = type;
        return this;
    }

    static Dictionary<object, Interpolator<T>> _stackInterpolators = new Dictionary<object,Interpolator<T>>();
    public Interpolator<T> ObjectStack(object stack)
    {
        if (!_stackInterpolators.ContainsKey(stack))
        {
            _stackInterpolators.Add(stack, this);
            return this;
        }

        Interpolator<T> lastInter = _stackInterpolators[stack];
        if (!lastInter._isDone)
        {
            Stop();
            lastInter.whenDone += () => Play();
        }
        _stackInterpolators[stack] = this;
        return this;
    }

    public Interpolator<T> ObjectStackStart(object stack)
    {
        if (!_stackInterpolators.ContainsKey(stack))
        {
            _stackInterpolators.Add(stack, this);
            return this;
        }

        Interpolator<T> lastInter = _stackInterpolators[stack];
        if (!lastInter._isUpdating && !lastInter._isDone)
        {
            Stop();
            lastInter.whenStart += () => Play();
        }
        _stackInterpolators[stack] = this;
        return this;
    }

    bool _isDone, _isUpdating = true;
    public override void Update()
    {
        if (!_isUpdating)
            return;
        if (_haveToNullCheck && _nullCheck == null)
        {
            _isDone = true;
            return;
        }
        var delta = Time.deltaTime;
        if (_delay > 0f)
        {
            _delay -= delta;
            if (_delay > 0f) return;
            delta = -_delay;
        }
        var prevValue = _curValue;
        var x = _t / _over;
        var fx = Fx(x);

        var deltaF = fx - _lastF;
        if (_onDeltaSignChange != null)
        {
            if (deltaF * _lastDeltaF < 0)
                _onDeltaSignChange();
        }

        _curValue = _interpolateFunc(_from, _to, fx);

        if (_allowedDeltaFrom > 0f)
        {
            if (fx < _allowedDeltaFrom)
                _curValue = _from;
            else
            {
                var divisor = _allowedDeltaTo - _allowedDeltaFrom;
                _curValue = _interpolateFunc(_from, _to,
                    (fx - _allowedDeltaFrom) / divisor);
            }
        }
        if (_allowedDeltaTo < 1f)
        {
            if (fx > _allowedDeltaTo)
                _curValue = _to;
        }
        
        _curValueDelta = _subtractFunc(_curValue, prevValue);
        
        _passDelta?.Invoke(_curValueDelta);
        _passValue?.Invoke(_curValue);
        
        _lastDeltaF = deltaF;
        _lastF = fx;

        if (_t == 0f)
            whenStart?.Invoke();

        if (_t == _over)
        {
            _isDone = true;
            whenDone?.Invoke();
            return;
        }

        _t += delta;
        if (_t > _over)
        {
            _t = _over;
        }
    }

    float Fx(float x)
    {
        var fx = 0f;
        switch (_interpolationType)
        {
            case InterpolationType.Linear:
                fx = x;
                break;
            case InterpolationType.Square:
                fx = x * x;
                break;
            case InterpolationType.InvSquare:
                fx = 1 - (1 - x) * (1 - x);
                break;
            case InterpolationType.OverflowReturn:
                fx = x * x * 0.194638370849f + Mathf.Sin(x * 2.3f) * 1.08f;
                break;
        }

        return fx;
    }

    public override bool IsDone()
    {
        return _isDone;
    }
}