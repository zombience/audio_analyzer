using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class AudioParticleBase : AudioFXBase
{

    protected ParticleSystem ps;
    protected ParticleSystem.Particle[] particles;
    

    protected virtual void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }


    protected ParticleSystem.Particle[] ProcessParticles()
    {
        int pCount = ps.GetParticles(particles);

        // do something to each particle, or emit particles, etc. 
        return particles;
    }

}
