using UnityEngine;

public class FieldCell : MonoBehaviour
{
    public int X, Y;
    public Shape occupiedBy;
    public FieldMatrix matrix;

    void SetCoords(int x, int y)
    {
        X = x;
        Y = y;
        transform.localPosition = matrix.ZeroPos + new Vector2(x, y);
    }

    public void Destroy()
    {
        DestroyImmediate(gameObject);
    }

    public static FieldCell Create(FieldMatrix matrix, int x, int y)
    {
        var go = Instantiate(Prefabs.Instance.fieldCell, matrix.transform);
        var fc = go.GetComponent<FieldCell>();
        fc.matrix = matrix;
        fc.SetCoords(x, y);
        return fc;
    }
}