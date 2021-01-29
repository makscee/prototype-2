using UnityEngine;

public class ShapeCellShaderPattern : MonoBehaviour
{
    [SerializeField] SpriteRenderer background;

    void Start()
    {
        SetMaterial(GetComponentInParent<PatternMaterialProvider>().material);
    }

    void SetMaterial(Material material)
    {
        background.material = material;
    }
}