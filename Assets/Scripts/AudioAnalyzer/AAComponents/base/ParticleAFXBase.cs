using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAFXBase : AFXBase
{
    protected ParticleSystem ps;
    protected ParticleSystem.EmissionModule em;
    protected ParticleSystem.MainModule psMain;
    protected ParticleSystem.Particle[] particles;

    protected virtual void Start()
    {
        ps = GetComponent<ParticleSystem>();
        em = ps.emission;
        psMain = ps.main;
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
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