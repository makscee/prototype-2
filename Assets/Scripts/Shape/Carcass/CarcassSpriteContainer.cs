using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class CarcassSpriteContainer : MonoBehaviour
{
    [SerializeField] bool[] closedSides = new bool[4];
    int ClosedSidesAmount => closedSides.Count(b => b);

    public FrontCarcassSprite[] frontSprites;
    public BackCarcassSprite[] backSprites;

    void OnValidate()
    {
        Refresh();
    }

    void Refresh()
    {
        frontSprites = GetComponentsInChildren<FrontCarcassSprite>();
        backSprites = GetComponentsInChildren<BackCarcassSprite>();
    }

    void Rotate()
    {
        transform.localRotation *= Quaternion.AngleAxis(-90f, Vector3.forward);
        var closed0 = closedSides[0];
        for (var i = 3; i > 0; i--)
            closedSides[i] = closedSides[i - 1];
        closedSides[3] = closed0;
    }

    public static CarcassSpriteContainer Create(ShapeCellCarcassObject carcass)
    {
        var closedSides = carcass.closedSides;
        var closedCount = closedSides.Count(b => b);
        var prefabsFiltered = Prefabs.Instance.carcassSpriteContainers.ToArray();
        switch (closedCount)
        {
            case 4:
                break;
            case 3:
                prefabsFiltered = prefabsFiltered.Where(container => container.ClosedSidesAmount < 4).ToArray();
                break;
            case 2:
                var adjacent = IsDoubleAdjacent(closedSides);
                prefabsFiltered = prefabsFiltered
                    .Where(container =>
                        container.ClosedSidesAmount == 2 && IsDoubleAdjacent(container.closedSides) == adjacent ||
                        container.ClosedSidesAmount < 2).ToArray();
                break;
            case 1:
                prefabsFiltered = prefabsFiltered.Where(container => container.ClosedSidesAmount < 2).ToArray();
                break;
        }

        var prefab = prefabsFiltered.Random();
        var result = Instantiate(prefab);
        
        var t = result.transform;
        t.SetParent(carcass.transform);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;

        for (var i = 0; i < 4; i++)
        {
            var fit = true;
            for (var j = 0; j < 4; j++)
            {
                if (!closedSides[j] && result.closedSides[j])
                {
                    fit = false;
                    break;
                }
            }

            if (fit)
                break;
            result.Rotate();
        }

        result.closedSides = closedSides;
        return result;
    }

    static bool IsDoubleAdjacent(bool[] closedSides)
    {
        for (var i = 0; i < 4; i++)
            if (closedSides[i] && closedSides[(i + 1) % 4])
                return true;
        return false;
    }
}