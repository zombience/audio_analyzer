using UnityEngine;
using System.Collections;

public class MoveByAmplitude : AudioFXBase 
{
	[SerializeField]
	protected Vector3 direction;

	protected Vector3 origin;


    public float debugRawLevel, debugInputLevel;
    

	#region Unity Methods
	void Start ()
	{
		origin = transform.position;
	}
	
	void Update () 
	{
		transform.position = origin + (direction * bandValue);
        debugRawLevel = AudioAnalyzer.GetRawOutput(band);
        debugInputLevel = AudioAnalyzer.GetScaledOutput(band, minOutput, maxOutput);
	}
	#endregion
}
