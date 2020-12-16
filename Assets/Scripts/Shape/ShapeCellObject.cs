using System;
using UnityEngine;

public class ShapeCellObject : MonoBehaviour
{
    public ShapeCell cell;
    public static ShapeCellObject Create(Vector2Int pos, Shape shape, ShapeCell cell)
    {
        var sco = Instantiate(Prefabs.Instance.shapeCellObject, shape.shapeObject.transform)
            .GetComponent<ShapeCellObject>();
        sco.transform.localPosition = (Vector2)pos;
        sco.cell = cell;
        return sco;
    }

    ShapeCellCarcassObject _carcassObject;
    CarcassSpriteContainer _carcassSpritesContainer;
    void Start()
    {
        if (_carcassSpritesContainer == null)
        {
            _carcassObject = GetComponentInChildren<ShapeCellCarcassObject>();
            InitCarcass();
        }
    }

    void InitCarcass()
    {
        bool[] closedSides = new bool[4];
        for (var i = 0; i < 4; i++)
        {
            var dir = Utils.CoordsFromDirInt(i);
            closedSides[i] = cell.shape[cell.LocalPos + dir] == null;
        }
        _carcassObject.closedSides = closedSides;
        _carcassObject.Refresh();
        _carcassSpritesContainer = CarcassSpriteContainer.Create(_carcassObject);
    }
}