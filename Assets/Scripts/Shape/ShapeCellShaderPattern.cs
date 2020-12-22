using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

[ExecuteAlways]
public class ShapeCellShaderPattern : MonoBehaviour
{
    [SerializeField] SpriteRenderer background;
    public Vector2 shaderPosition, shaderSize;
    static readonly int PositionProperty = Shader.PropertyToID("_Position");
    static readonly int SizeProperty = Shader.PropertyToID("_Size");
    static readonly int Color0Property = Shader.PropertyToID("_Color0");
    static readonly int Color1Property = Shader.PropertyToID("_Color1");

    void Start()
    {
        ObtainPosition();
        SetMaterial(GlobalConfig.Instance.shaderPatternMaterial);
    }

    void OnValidate()
    {
        // SetMaterial(GlobalConfig.Instance.shaderPatternMaterial);
        // ObtainPosition();
    }

    void ObtainPosition()
    {
        shaderPosition = Vector2.zero;
        shaderSize = Vector2.one;
        var cellObject = GetComponentInParent<ShapeCellObject>();
        var matrix = GetComponentInParent<FieldMatrix>();
        if (!cellObject || !matrix) return;
        var cell = cellObject.cell;
        
        shaderSize = Vector2.one / matrix.size;
        shaderPosition = cell.originalPos * shaderSize;
    }

    void SetMaterial(Material material)
    {
        background.material = material;
        Material mat;
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null && prefabStage.mode == PrefabStage.Mode.InIsolation)
        {
            mat = background.sharedMaterial;
        }
        else
        {
            mat = background.material;
        }
        mat.SetVector(PositionProperty, shaderPosition);
        mat.SetVector(SizeProperty, shaderSize);
        mat.SetColor(Color0Property, GlobalConfig.Instance.palette2);
        mat.SetColor(Color1Property, GlobalConfig.Instance.palette3);
    }
    
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