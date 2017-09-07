using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioAnalyzer
{
	public class SpiralParticleAFX : ParticleAFXBase
	{
		[SerializeField, Range(0, 1f)]
		float spiralForce	= .2f;

		[SerializeField, Range(0, 1.5f)]
		float coherence		= 2f;

		[SerializeField]
		Vector3 axis = Vector3.zero;

		[SerializeField]
		bool rotateAxis;

		[SerializeField, Range(.1f, 2f)]
		float rotateSpeed = 1f;

		float value { get { return band.bandValue + .5f; } }

		protected override void ProcessParticles()
		{
			int pCount = ps.GetParticles(particles);

			if(rotateAxis)
			{
				axis.x = Mathf.Sin(Time.time * rotateSpeed);
				axis.z = Mathf.Cos(Time.time * rotateSpeed);
			}

			for (int i = 1; i < pCount; i++)
			{
				int target = i == 0 ? pCount - 1 : i - 1;
				
				Vector3 dir				= (particles[target].position - particles[i].position).normalized;
				Vector3 cross			= Vector3.Cross(axis, dir).normalized;
				particles[i].position	+= (cross) * spiralForce * value;
				int prev = i == 0 ? pCount - 1 : i - 1;
				particles[i].position	+= (particles[prev].position - particles[i].position).normalized * coherence * value;
				particles[i].position	+= dir * coherence * value;
			}

			ps.SetParticles(particles, pCount);
		}
	}
}