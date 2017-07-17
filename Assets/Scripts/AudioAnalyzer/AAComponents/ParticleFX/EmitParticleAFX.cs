using UnityEngine;
namespace AudioAnalyzer
{
	public class EmitParticleAFX : ParticleAFXBase
	{

		[SerializeField]
		protected int emitCount = 10;

		[SerializeField]
		protected float threshold = 5f, pulseLength = 2f;
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
			if (curTime < 0 && band.bandValue > threshold)
			{
				curTime = pulseLength;
				ps.Emit(emitCount);
			}

			curTime -= Time.deltaTime;
		}
	}
}