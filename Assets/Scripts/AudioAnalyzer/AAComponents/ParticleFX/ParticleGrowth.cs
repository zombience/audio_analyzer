using UnityEngine;


namespace AudioAnalyzer
{
	public class ParticleGrowth : ParticleAFXBase
	{
		[SerializeField]
		float growthSpeed = .75f;
		[SerializeField, Range(0.05f, 0.95f)]
		float threshold = .3f;

		[SerializeField]
		bool useTargetColor;

		[SerializeField]
		Color targetColor = Color.cyan;


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
						if (useTargetColor)
						{
							particles[i].startColor = targetColor * band.bandValue;
						}
					}
				}
				ps.SetParticles(particles, pCount);
			}
		}
	}
}
