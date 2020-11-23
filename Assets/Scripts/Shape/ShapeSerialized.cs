using System;
using System.Collections.Generic;

[Serializable]
public class ShapeSerialized
{
    public int sizeX, sizeY;
    public List<CellSerialized> cells = new List<CellSerialized>();

    public ShapeSerialized(int sizeX, int sizeY)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
    }

    public static ShapeSerialized CreateFromString(string[] shapeCells)
    {
        var shapeSerialized = new ShapeSerialized(shapeCells[0].Length, shapeCells.Length);
        for (var i = 0; i < shapeCells.Length; i++)
        {
            var y = shapeCells.Length - i - 1;
            for (var x = 0; x < shapeCells[0].Length; x++)
            {
                if (shapeCells[i][x] == '-') continue;
                shapeSerialized.cells.Add(new CellSerialized(x, y));
            }
        }
        return shapeSerialized;
    }
}