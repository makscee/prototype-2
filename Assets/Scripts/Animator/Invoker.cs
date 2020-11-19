using System;
using UnityEngine;

public class Invoker : IUpdateable
{
    Action _action;
    public Invoker(Action action)
    {
        _action = action;
    }
    public void Update()
    {
        _in -= Time.deltaTime;
        if (_in < 0f)
        {
            _action();
            if (_repeat > 0)
            {
                _in = _repeatIn;
                _repeat--;
            }
        }
    }

    float _in;
    public Invoker In(float t)
    {
        _in = t; 
        return this;
    }

    int _repeat = 0;
    public Invoker Repeat(int times)
    {
        _repeat = times;
        return this;
    }

    float _repeatIn;
    public Invoker RepeatIn(float t)
    {
        _repeatIn = t;
        return this;
    }

    public bool IsDone()
    {
        return _in < 0f && _repeat == 0;
    }
}