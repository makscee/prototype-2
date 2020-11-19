using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Object = System.Object;

public enum InterpolationType
{
    Square, InvSquare, Linear, OverflowReturn
}

[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
public class Interpolator<T> : IUpdateable
{
    float _over, _t, _delay;
    T _from, _to, _cur;
    Func<T, T, T> _addFunc, _subtractFunc;
    Func<T, float, T> _multiplyFunc;
    public Interpolator(T from, T to, float over, Func<T, float, T> multiplyFunc, Func<T, T, T> addFunc, Func<T, T, T> subtractFunc)
    {
        _from = from;
        _cur = from;
        _to = to;
        _over = over;
        _multiplyFunc = multiplyFunc;
        _addFunc = addFunc;
        _subtractFunc = subtractFunc;
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

    Action _onDeltaSignChange;
    float _lastDeltaF, _lastF;
    public Interpolator<T> OnDeltaSignChange(Action action)
    {
        _onDeltaSignChange = action;
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
            lastInter.whenDone += Resume;
        }
        _stackInterpolators[stack] = this;
        return this;
    }

    void Resume()
    {
        _isUpdating = true;
    }

    void Stop()
    {
        _isUpdating = false;
    }

    bool _isDone, _isUpdating = true;
    public void Update()
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
        var before = _cur;
        var x = _t / _over;
        var f = 0f;

        switch (_interpolationType)
        {
            case InterpolationType.Linear:
                f = x;
                break;
            case InterpolationType.Square:
                f = x * x;
                break;
            case InterpolationType.InvSquare:
                f = 1 - (1 - x) * (1 - x);
                break;
            case InterpolationType.OverflowReturn:
                f = x * x * 0.194638370849f + Mathf.Sin(x * 2.3f) * 1.08f;
                break;
        }

        if (_onDeltaSignChange != null)
        {
            var deltaF = f - _lastF;
            if (deltaF * _lastDeltaF < 0)
                _onDeltaSignChange();
            _lastDeltaF = deltaF;
            _lastF = f;
        }
        
        _cur = _addFunc(_from, _multiplyFunc(_subtractFunc(_to, _from), f));
        _passDelta?.Invoke(_subtractFunc(_cur, before));
        _passValue?.Invoke(_cur);
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

    public bool IsDone()
    {
        return _isDone;
    }
}