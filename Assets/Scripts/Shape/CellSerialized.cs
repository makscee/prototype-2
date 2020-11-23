using System;

[Serializable]
public class CellSerialized
{
    public int x, y;

    public CellSerialized(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}