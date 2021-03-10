using System;
using UnityEngine;

public class ShapeCellParticleSystem : MonoBehaviour
{
    const float DefaultRateOverDistance = 5;
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

    public void SetParticlesAmount(float value)
    {
        var particlesEmission = particles.emission;
        particlesEmission.rateOverDistance = Mathf.Lerp(0f, DefaultRateOverDistance, value);
    }
}