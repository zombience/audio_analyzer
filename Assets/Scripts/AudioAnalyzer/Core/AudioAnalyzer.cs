using UnityEngine;


namespace AudioAnalyzer
{
	[RequireComponent(typeof(AudioSource), typeof(FilterBands))]
	public class AudioAnalyzer : MonoBehaviour
	{
		static FilterBands bands;

		#region Unity Methods
		void Awake()
		{
			bands = GetComponent<FilterBands>();
		}

		#endregion


		#region API
		/// <summary>
		/// GetScaledOutput will return a value between desired range
		/// values are scaled from -60dB min 0dB max (digital scale)
		/// if input values should fall outside that range, output values will not be clamped
		/// </summary>
		/// <param name="band">if band is out of range, value of the next lowest band will be returned</param>
		/// <param name="targetMin">output minimum</param>
		/// <param name="targetMax">output maximum</param>
		/// <returns></returns>
		static public float GetScaledOutput(int band, float targetMin, float targetMax)
		{
			return bands.GetScaledOutput(band, targetMin, targetMax);
		}

		/// <summary>
		/// GetRawDb will return unscaled dB output of band analysis
		/// </summary>
		/// <param name="band"></param>
		/// <returns></returns>
		static public float GetRawOutput(int band)
		{
			return bands.GetRawOutput(band);
		}
		#endregion
	}
}