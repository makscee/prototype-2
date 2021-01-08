using UnityEngine;

public class OutlineMaskObject : MonoBehaviour
{
    [SerializeField] bool[] alwaysClosedSides;
    public void SetClosedSides(bool[] sides)
    {
        var closedSides = new bool[4];
        for (var i = 0; i < 4; i++)
            closedSides[i] = sides[i] || alwaysClosedSides[i];
        
        var size = GlobalConfig.Instance.thickness;
        var sizeVec = new Vector3(size, size, size);
        var position = Vector3.zero;
        var scale = Vector3.one - sizeVec;
        for (var i = 0; i < 4; i++)
        {
            if (!closedSides[i])
            {
                var dir = (Vector3) Utils.CoordsFromDir(i);
                position += dir * size / 4;
                scale += new Vector3(Mathf.Abs(dir.x), Mathf.Abs(dir.y)) * size / 2;
            }
        }

        transform.localPosition = position;
        transform.localScale = scale;
    }
}