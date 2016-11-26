using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class AudioParticleBase : AudioFXBase
{

    protected ParticleSystem ps;
    protected ParticleSystem.EmissionModule em;
    protected ParticleSystem.Particle[] particles;

    protected virtual void Start()
    {
        ps = GetComponent<ParticleSystem>();
        em = ps.emission;
        particles = new ParticleSystem.Particle[ps.maxParticles];
    }

    protected virtual void Update()
    {
        ProcessParticles();
    }
    
    protected virtual void ProcessParticles()
    {
        int pCount = ps.GetParticles(particles);
        // do something to each particle, or emit particles, etc. 
        ps.SetParticles(particles, pCount);
    }
}