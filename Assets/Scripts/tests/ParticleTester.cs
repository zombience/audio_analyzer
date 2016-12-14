using UnityEngine;

public class ParticleTester : ParticleAFXBase
{
    [SerializeField]
    protected float growthSpeed = .75f, threshold = .3f;

    [SerializeField]
    protected Color targetColor = Color.cyan;

    protected override void ProcessParticles()
    {
        if(bandValue > threshold)
        {
            int pCount = ps.GetParticles(particles);
            for(int i = 0; i < pCount; i++)
            {
                if (particles[i].remainingLifetime > psMain.startLifetime.constant * .9f)
                {
                    particles[i].startSize += growthSpeed;
                    particles[i].startColor = targetColor * bandValue;
                }
            }
            ps.SetParticles(particles, pCount);
        }
    }
}
