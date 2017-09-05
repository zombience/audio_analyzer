using UnityEngine;
using System.Collections;

namespace AudioAnalyzer
{

	public class WanderAFX : AFXNormalizedBase
	{
		[SerializeField]
		protected float headingChangeSpeed = .5f,
			maxWanderRange = 5f;

		protected Vector3 origin, dir;

		protected void Start()
		{
			origin	= transform.position;
			dir		= transform.forward;
		}

		protected void Update()
		{

			Vector3 newDir = Random.onUnitSphere;

			dir = Vector3.Lerp(dir, newDir, headingChangeSpeed * band.bandValue);
			dir *= band.bandValue;

			if (Vector3.Distance(transform.position + dir, origin) > maxWanderRange)
			{
				dir = (origin - transform.position).normalized;
			}

			transform.rotation = Quaternion.Lerp(
				transform.rotation, dir != Vector3.zero ? 
				Quaternion.LookRotation(-dir)			: 
				Quaternion.identity, headingChangeSpeed * band.bandValue);

			transform.position += dir;
		}
	}
}