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
        if (prefabStage != null && prefabStage.mode == PrefabStage.Mode.InIsolation || !Application.isPlaying)
        {
            mat = background.sharedMaterial;
        }
        else
        {
            mat = background.material;
        }
        mat.SetVector(PositionProperty, shaderPosition);
        mat.SetVector(SizeProperty, shaderSize);
        mat.SetColor(Color0Property, GlobalConfig.Instance.palette1);
        mat.SetColor(Color1Property, GlobalConfig.Instance.palette3);
    }
}