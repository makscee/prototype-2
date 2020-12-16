using UnityEngine;

public class OutlineMaskObject : MonoBehaviour
{
    public void SetClosedSides(bool[] sides)
    {
        var size = GlobalConfig.Instance.outlineThickness;
        var sizeVec = new Vector3(size, size, size);
        var position = Vector3.zero;
        var scale = Vector3.one - sizeVec;
        for (var i = 0; i < 4; i++)
        {
            if (!sides[i])
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