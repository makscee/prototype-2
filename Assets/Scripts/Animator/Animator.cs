using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Animator
{
    static List<IUpdateable> _updateables = new List<IUpdateable>();
    static List<IUpdateable> _updateablesToAdd = new List<IUpdateable>();
    public static Interpolator<float> Interpolate(float from, float to, float over)
    {
        var result = new Interpolator<float>(from, to, over, 
            (v, f) => v * f, (v1, v2) => v1 + v2, (v1, v2) => v1 - v2);
        _updateablesToAdd.Add(result);
        return result;
    }
    public static Interpolator<Vector2> Interpolate(Vector2 from, Vector2 to, float over)
    {
        var result = new Interpolator<Vector2>(from, to, over, 
            (v, f) => v * f, (v1, v2) => v1 + v2, (v1, v2) => v1 - v2);
        _updateablesToAdd.Add(result);
        return result;
    }
    public static Interpolator<Vector3> Interpolate(Vector3 from, Vector3 to, float over)
    {
        var result = new Interpolator<Vector3>(from, to, over, 
            (v, f) => v * f, (v1, v2) => v1 + v2, (v1, v2) => v1 - v2);
        _updateablesToAdd.Add(result);
        return result;
    }
    public static Interpolator<Color> Interpolate(Color from, Color to, float over)
    {
        var result = new Interpolator<Color>(from, to, over, 
            (v, f) => v * f, (v1, v2) => v1 + v2, (v1, v2) => v1 - v2);
        _updateablesToAdd.Add(result);
        return result;
    }

    public static Invoker Invoke(Action action)
    {
        var result = new Invoker(action);
        _updateablesToAdd.Add(result);
        return result;
    }

    public static void Update()
    {
        for (var i = 0; i < _updateables.Count; i++)
        {
            _updateables[i].Update();
            if (_updateables[i].IsDone())
            {
                _updateables.RemoveAt(i);
                i--;
            }
        }

        foreach (var updateable in _updateablesToAdd)
        {
            _updateables.Add(updateable);
        }
        _updateablesToAdd.Clear();
    }
}