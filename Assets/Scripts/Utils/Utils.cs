using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void CoordsFromDir(int dir, out int x, out int y)
    {
        switch (dir)
        {
            case 0:
                x = 0;
                y = 1;
                return;
            case 1:
                x = 1;
                y = 0;
                return;
            case 2:
                x = 0;
                y = -1;
                return;
            case 3:
                x = -1;
                y = 0;
                return;
            default:
                throw new Exception($"Wrong dir passed: {dir}");
        }
    }

    public static Vector2 CoordsFromDir(int dir)
    {
        CoordsFromDir(dir, out var x, out var y);
        return new Vector2(x, y);
    }

    public static Vector2Int CoordsFromDirInt(int dir)
    {
        CoordsFromDir(dir, out var x, out var y);
        return new Vector2Int(x, y);
    }

    public static Vector2Int Rotate90(this Vector2Int v, bool clockwise, int count = 1)
    {
        for (var i = 0; i < count; i++)
            v = clockwise ? new Vector2Int(v.y, -v.x) : new Vector2Int(-v.y, v.x);
        return v;
    }
    public static Vector2 Rotate90(this Vector2 v, bool clockwise, int count = 1)
    {
        for (var i = 0; i < count; i++)
            v = clockwise ? new Vector2(v.y, -v.x) : new Vector2(-v.y, v.x);
        return v;
    }

    public static Vector2Int Abs(this Vector2Int v)
    {
        v.x = Mathf.Abs(v.x);
        v.y = Mathf.Abs(v.y);
        return v;
    }

    public static Vector2 Abs(this Vector2 v)
    {
        v.x = Mathf.Abs(v.x);
        v.y = Mathf.Abs(v.y);
        return v;
    }

    public static T Random<T>(this T[] obj)
    {
        return obj[UnityEngine.Random.Range(0, obj.Length)];
    }

    public static int DirFromCoords(int x, int y)
    {
        if (x == 0 && y > 0) return 0;
        if (x > 0 && y == 0) return 1;
        if (x == 0 && y < 0) return 2;
        if (x < 0 && y == 0) return 3;
        throw new Exception($"Wrong coords for dir passed {x} {y}");
    }

    public static int DirFromCoords(Vector2 v)
    {
        int x = (int) v.x, y = (int) v.y;
        return DirFromCoords(x, y);
    }

    public static void Shuffle<T>(ref T[] a)
    {
        for (var i = 0; i < a.Length; i++)
        {
            var ind1 = UnityEngine.Random.Range(0, a.Length);
            var ind2 = UnityEngine.Random.Range(0, a.Length);
            var t = a[ind2];
            a[ind2] = a[ind1];
            a[ind1] = t;
        }
    }
    
    public static Color ChangeAlpha(this Color c, float alpha)
    {
        c.a = alpha;
        return c;
    }
    
    public static Color LerpAlpha(this Color c, float a, float b, float t)
    {
        c.a = Mathf.Lerp(a, b, t);
        return c;
    }

    static Dictionary<Transform, RectTransform> _canvasTransforms = new Dictionary<Transform, RectTransform>();
    public static Vector2 ScaledScreenCoords(Vector2 v, Transform transform, bool reversed = false)
    {
        if (!_canvasTransforms.ContainsKey(transform))
            _canvasTransforms.Add(transform, transform.GetComponentInParent<Canvas>().GetComponent<RectTransform>());
        var size = _canvasTransforms[transform].rect.size;
        Vector2 scale;
        if (reversed)
            scale = new Vector2(Screen.width, Screen.height) / size;
        else scale = size / new Vector2(Screen.width, Screen.height);
        return v * scale;
    }
}