using UnityEngine;

public class ShapeCellShaderPattern : MonoBehaviour
{
    [SerializeField] SpriteRenderer background;

    void Start()
    {
        SetMaterial(GetComponentInParent<PatternMaterialProvider>().Material);
    }

    void SetMaterial(Material material)
    {
        background.material = material;
    }
}