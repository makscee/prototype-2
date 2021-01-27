using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

[ExecuteAlways]
public class ShapeCellShaderPattern : MonoBehaviour
{
    [SerializeField] SpriteRenderer background;

    void Start()
    {
        SetMaterial(GetComponentInParent<PatternMaterialProvider>().material);
    }

    void OnValidate()
    {
        // SetMaterial(GlobalConfig.Instance.shaderPatternMaterial);
        // ObtainPosition();
    }

    void SetMaterial(Material material)
    {
        background.material = material;
        // Material mat = material;
        // var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        // if (prefabStage != null && prefabStage.mode == PrefabStage.Mode.InIsolation || !Application.isPlaying)
        // {
        //     mat = background.sharedMaterial;
        // }
        // else
        // {
        //     mat = background.material;
        // }
        // mat.SetVector(PositionProperty, shaderPosition);
        // mat.SetVector(SizeProperty, shaderSize);
        // mat.SetColor(Color0Property, GlobalConfig.Instance.palette1);
        // mat.SetColor(Color1Property, GlobalConfig.Instance.palette3);
    }
}