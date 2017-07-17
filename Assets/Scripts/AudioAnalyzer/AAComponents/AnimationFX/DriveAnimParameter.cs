using System.Collections.Generic;
using UnityEngine;

namespace AudioAnalyzer
{
	public class DriveAnimParameter : AnimationAFXBase
	{

		[SerializeField]
		protected string targetParam = "animSpeed";


		protected void Update()
		{
			anim.SetFloat(targetParam, band.bandValue);

		}
	}
}