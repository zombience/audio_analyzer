using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif



namespace AudioAnalyzer
{
	[System.Serializable]
	public class BandValue
	{
		// choose which audio band to drive interactions
		[SerializeField]	protected int band;

		[SerializeField]	protected bool easeFall;

		// set the range of min/max to achieve desired range of effect
		[SerializeField]
		protected float
			minOutput	= 0f,
			maxOutput	= 1f,
			offset		= 0f;

		[SerializeField, Range(0.05f, 0.95f)]
		protected float fallRate = .9f;

		public float	bandValue
		{
			get
			{
				float newVal = AudioAnalyzer.GetScaledOutput(band, minOutput, maxOutput);
				if (easeFall && newVal < _bandValue)
				{
					_bandValue = Mathf.Lerp(_bandValue, newVal, fallRate);
				}
				else
				{
					_bandValue = newVal;
				}
				return _bandValue + offset;
			}
		}

		protected float _bandValue;
	}
}


