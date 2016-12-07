using UnityEngine;
using System.Collections;

public class MoveByAmplitude : AudioFXBase 
{
	[SerializeField]
	protected Vector3 direction;

	protected Vector3 origin;

	#region Unity Methods
	void Start ()
	{
		origin = transform.position;
	}
	
	void Update () 
	{
		transform.position = origin + (direction * bandValue);
	}
	#endregion
}
