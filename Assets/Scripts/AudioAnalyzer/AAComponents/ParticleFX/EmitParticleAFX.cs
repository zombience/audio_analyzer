using UnityEngine;
using System.Collections;

public class EmitParticleAFX : ParticleAFXBase
{

    [SerializeField]
    protected int emitCount = 10;

    [SerializeField]
    protected float threshold = 5f, pauseTime = 2f;
    float curTime;

    protected bool canTrigger = true;

    protected override void Start()
    {
        base.Start();
        em.enabled = false;
        psMain.loop = false;
    }
        
    protected override void ProcessParticles()
    {
        
        if(curTime < 0 && bandValue > threshold)
        {
            curTime = pauseTime;
            ps.Emit(emitCount);
        }

        curTime -= Time.deltaTime;
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
