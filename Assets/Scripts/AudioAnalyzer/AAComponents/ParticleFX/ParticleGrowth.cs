using UnityEngine;


namespace AudioAnalyzer
{
	public class ParticleGrowth : ParticleAFXBase
	{
		[SerializeField]
		protected float growthSpeed = .75f;
		[SerializeField, Range(0.05f, 0.95f)]
		protected float threshold = .3f;

		[SerializeField]
		protected Color targetColor = Color.cyan;

		protected override void ProcessParticles()
		{
			if (band.bandValue > threshold)
			{
				int pCount = ps.GetParticles(particles);
				for (int i = 0; i < pCount; i++)
				{
					if (particles[i].remainingLifetime > psMain.startLifetime.constant * .9f)
					{
						particles[i].startSize += growthSpeed;
						particles[i].startColor = targetColor * band.bandValue;
					}
				}
				ps.SetParticles(particles, pCount);
			}
		}
	}
}
