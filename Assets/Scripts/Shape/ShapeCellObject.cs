using System;
using UnityEngine;

public class ShapeCellObject : MonoBehaviour
{
    public ShapeCell cell;
    public ShapeCellSidesContainer sidesContainer;
    public ShapeCellParticleSystem particles;
    public Shape shape;
    public static ShapeCellObject Create(Vector2Int pos, Shape shape, ShapeCell cell)
    {
        var sco = Instantiate(Prefabs.Instance.shapeCellObject, shape.shapeObject.transform)
            .GetComponent<ShapeCellObject>();
        sco.shape = shape;
        sco.transform.localPosition = (Vector2)pos;
        sco.cell = cell;
        return sco;
    }

    void Start()
    {
        InitInsides();
        InitParticles();
    }

    void InitParticles()
    {
        particles = ShapeCellParticleSystem.Create(this);
        var fieldSize = shape.Field.Size;
        const float sizeLimit = 6;
        if (fieldSize > sizeLimit)
        {
            var t = sizeLimit * sizeLimit / (fieldSize * fieldSize);
            particles.SetParticlesAmount(t);
        }
    }

    public void SetEnabled(bool value)
    {
        gameObject.SetActive(value);
        if (particles != null)
            particles.gameObject.SetActive(value);
    }

    void OnDestroy()
    {
        if (particles != null)
            Destroy(particles.gameObject);
    }

    public void InitInsides()
    {
        var surroundingCells = new bool[3, 3];
        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                // surroundingCells[x + 1, y + 1] = cell.shape[cell.LocalPos + new Vector2Int(x, y)] != null;
                
                surroundingCells[x + 1, y + 1] = shape[
                cell.LocalPos + new Vector2Int(x, y).Rotate90(true,
                shape.Rotation - shape.originalRotation)] != null;
            }
        }

        sidesContainer.surroundingCells = surroundingCells;
        sidesContainer.Refresh();
    }
}