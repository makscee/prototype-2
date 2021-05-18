using System;
using System.Collections.Generic;
using UnityEngine;

public static class Animator
{
    static List<OwnedUpdatable> _updateables = new List<OwnedUpdatable>();
    static List<OwnedUpdatable> _updateablesToAdd = new List<OwnedUpdatable>();
    static List<OwnedUpdatable> _updateablesToRemove = new List<OwnedUpdatable>();
    public static Interpolator<float> Interpolate(float from, float to, float over)
    {
        var result = new Interpolator<float>(from, to, over, 
            (v1, v2, t) => v1 + (v2 - v1) * t,
            (v1, v2) => v1 - v2
            );
        _updateablesToAdd.Add(result);
        return result;
    }
    public static Interpolator<Vector2> Interpolate(Vector2 from, Vector2 to, float over)
    {
        var result = new Interpolator<Vector2>(from, to, over, Vector2.LerpUnclamped,
            (v1, v2) => v1 - v2);
        _updateablesToAdd.Add(result);
        return result;
    }
    public static Interpolator<Vector3> Interpolate(Vector3 from, Vector3 to, float over)
    {
        var result = new Interpolator<Vector3>(from, to, over, Vector3.LerpUnclamped,
            (v1, v2) => v1 - v2);
        _updateablesToAdd.Add(result);
        return result;
    }
    // public static Interpolator<Quaternion> Interpolate(Quaternion from, Quaternion to, float over)
    // {
    //     var result = new Interpolator<Quaternion>(from, to, over, Quaternion.LerpUnclamped);
    //     _updateablesToAdd.Add(result);
    //     return result;
    // }
    public static Interpolator<Color> Interpolate(Color from, Color to, float over)
    {
        var result = new Interpolator<Color>(from, to, over, Color.LerpUnclamped,
            (v1, v2) => v1 - v2);
        _updateablesToAdd.Add(result);
        return result;
    }

    public static Invoker Invoke(Action action)
    {
        var result = new Invoker(action);
        _updateablesToAdd.Add(result);
        return result;
    }

    public static void ClearByOwner(object owner)
    {
        for (var i = 0; i < _updateables.Count; i++)
            if (_updateables[i].IsOwnedBy(owner))
                _updateablesToRemove.Add(_updateables[i]);

        for (var i = 0; i < _updateablesToAdd.Count; i++)
            if (_updateablesToAdd[i].IsOwnedBy(owner))
                _updateablesToAdd.RemoveAt(i--);
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

        foreach (var updateable in _updateablesToRemove)
        {
            _updateables.Remove(updateable);
        }
        _updateablesToRemove.Clear();

        foreach (var updateable in _updateablesToAdd)
        {
            _updateables.Add(updateable);
        }
        _updateablesToAdd.Clear();
    }

    public static void Reset()
    {
        _updateables.Clear();
        _updateablesToAdd.Clear();
    }
}