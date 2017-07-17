using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AudioAnalyzer
{
	public class ParticleTester : ParticleAFXBase
	{
		[SerializeField]
		protected float growthSpeed = .75f, threshold = .3f;

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

//#if UNITY_EDITOR
//[CustomEditor(typeof(ParticleTester))]
//public class ParticleTesterEditor : AFXBaseEditor
//{
//    protected override void OnEnable()
//    {
//        base.OnEnable();
//    }

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//    }
//}

//#endif