using UnityEngine;

namespace AudioAnalyzer
{
	public class LightIntensityAFX : AFXBase
	{


		[SerializeField]
		protected Color high = Color.white, low = Color.black;

		[SerializeField]
		protected bool useIntensity, useColor;

		[SerializeField]
		protected float intensityScale = 2f;

		protected Light targetLight;


		protected void Start()
		{
			targetLight = GetComponent<Light>();
		}

		protected void Update()
		{
			if (useIntensity) targetLight.intensity = band.bandValue * intensityScale;
			if (useColor) targetLight.color = Color.Lerp(low, high, band.bandValue);
		}
	}
}