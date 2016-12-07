using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(AudioSource))]
public class AudioAnalyzer : MonoBehaviour 
{

    static EQBands bands;

	#region Unity Methods
	void Awake() 
	{
        bands = FindObjectOfType<EQBands>();
        if(!bands)
        {
            throw new System.NullReferenceException("Audio Analyzer requires FilterBands to exist on an audio source or audio listener in the scene");
        }
    }

#endregion


#region API
    /// <summary>
    /// GetScaledOutput will return a value between desired range
    /// values are scaled from -60dB min 0dB max (digital scale)
    /// if input values should fall outside that range, output values will not be clamped
    /// </summary>
    /// <param name="listenBand">if band is out of range, value of the next lowest band will be returned</param>
    /// <param name="targetMin">output minimum</param>
    /// <param name="targetMax">output maximum</param>
    /// <returns></returns>
	public static float GetScaledOutput(int listenBand, float targetMin, float targetMax)
	{
        return bands.GetScaledOutput(listenBand, targetMin, targetMax);
	}
    
    /// <summary>
    /// GetRawDb will return unscaled dB output of band analysis
    /// </summary>
    /// <param name="listenBand"></param>
    /// <returns></returns>
    public static float GetRawOutput(int listenBand)
    {
        return bands.GetRawOutput(listenBand);
    }
#endregion

}
