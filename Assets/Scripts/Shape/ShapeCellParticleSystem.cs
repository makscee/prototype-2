using System;
using UnityEngine;

public class ShapeCellParticleSystem : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;
    [SerializeField] Transform cell;

    public static ShapeCellParticleSystem Create(ShapeCellObject cell)
    {
        var scp = Instantiate(Prefabs.Instance.shapeCellParticles,
            GameManager.instance.shapeCellsParticlesContainer.transform).GetComponent<ShapeCellParticleSystem>();
        scp.cell = cell.transform;
        return scp;
    }

    void Update()
    {
        transform.position = cell.position;
        transform.localScale = cell.lossyScale;
    }

    public void EnableEmission(bool value)
    {
        var e = particles.emission;
        e.enabled = value;
    }
}