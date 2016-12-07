using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleChaser : AudioParticleBase
{

    [SerializeField]
    protected float speed = 2f, threshold = .5f, spiralForce = .2f;

    protected override void ProcessParticles()
    {
        if(bandValue < threshold) return;

        int pCount = ps.GetParticles(particles);
        for (int i = 1; i < pCount; i++)
        {
            int target = i == 0 ? pCount - 1 : i - 1;

            Vector3 dir = Vector3.zero;
            dir = (particles[target].position - particles[i].position).normalized;
            Vector3 cross = Vector3.Cross(particles[i].position, dir);
            particles[i].position += dir * speed;
            particles[i].position += cross * spiralForce;
        }

        ps.SetParticles(particles, pCount);
    }
}
