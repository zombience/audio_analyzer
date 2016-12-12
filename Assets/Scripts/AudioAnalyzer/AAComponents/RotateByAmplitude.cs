using UnityEngine;
using System.Collections;

public class RotateByAmplitude : AFXBase
{	
	#region vars
	[SerializeField]
	protected Vector3 axis;
	[SerializeField]
	protected float speed = 1, threshold = 1;
	[SerializeField]
	protected bool useThreshold;
		


	#endregion
	
	#region Unity methods
	void Start () 
	{	

	}
	
	void Update () 
	{
        float val = bandValue;
		if (useThreshold && val > threshold)
		{
			transform.Rotate(axis, speed * val);
		}
		else
		{
            transform.Rotate(axis, speed * val);
		}
	}
	#endregion
}
