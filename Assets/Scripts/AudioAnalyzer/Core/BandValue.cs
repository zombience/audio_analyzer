using UnityEngine;

namespace AudioAnalyzer
{
	/// <summary>
	/// BandValue returns a value between 0 and 1 for the given listen band
	/// </summary>
	[System.Serializable]
	public class BandValueNormalized
	{
		// choose which audio band to drive interactions
		[SerializeField, Range(0, 4)] protected int band;

		[SerializeField] protected bool easeFall;

		[SerializeField, Range(0.05f, 0.95f)]
		protected float fallRate = .9f;

		public float bandValue
		{
			get
			{
				float newVal = AudioAnalyzer.GetScaledOutput(band, 0, 1);
				if (easeFall && newVal < _bandValue)
				{
					_bandValue = Mathf.Lerp(_bandValue, newVal, fallRate);
				}
				else
				{
					_bandValue = newVal;
				}
				return _bandValue;
			}
		}

		protected float _bandValue;
	}

	/// <summary>
	/// BandValueExpandable returns a value between minOutput and maxOutput for the given listen band
	/// </summary>
	[System.Serializable]
	public class BandValueRange : BandValueNormalized
	{

		// set the range of min/max to achieve desired range of effect
		[SerializeField]
		protected float
			minOutput	= 0f,
			maxOutput	= 1f,
			offset		= 0f;

		new public float bandValue
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
	}
}


