using UnityEngine;
using System.Collections;

public class EmitByAmplitude : AudioParticleBase
{

    [SerializeField]
    protected int emitCount = 100;

    [SerializeField]
    protected float threshold = 5f, emissionRateMod = 10f,
        // hysteresis is the value below which particles over vulnerableAge will be killed
        hysteresis = 4f, 
        vulnerableAge = .5f;

    protected bool canTrigger = true;

    protected override void ProcessParticles()
    {
        if(canTrigger && bandValue > threshold)
        {
            ps.Emit(emitCount);
            canTrigger = false;
        }

        if(!canTrigger)
        {
            canTrigger = bandValue < hysteresis;
        }

        //if (particles.Length != ps.maxParticles) particles = new ParticleSystem.Particle[ps.maxParticles];
        //int count = ps.GetParticles(particles);

        //for (int i = 0; i < count; i++)
        //{
        //    particles[i].startSize = ps.startSize * bandValue;
        //}

        //ps.SetParticles(particles, particles.Length);
        //em.rate = bandValue * emissionRateMod;

    }
}
