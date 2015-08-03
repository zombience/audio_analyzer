using UnityEngine;
using System.Collections;

public class RotateByAmplitude : MonoBehaviour 
{
	
	#region vars
	[SerializeField]
	protected Vector3 axis;
	[SerializeField]
	protected float speed, threshold;
	[SerializeField]
	protected bool useThreshold;

	[SerializeField]
	[Range(0,3)]
	protected int listenBand;
		
	
	protected Transform trans;
	protected float val;
	#endregion
	
	#region Unity methods
	void Start () 
	{	
		trans = transform;
	}
	
	void Update () 
	{
		val = AudioAnalyzer.output[listenBand];
		if (useThreshold && val > threshold)
		{
			trans.Rotate(axis, speed * val);
		}
		else
		{
			trans.Rotate(axis, speed * val);
		}
	}
	#endregion
}
