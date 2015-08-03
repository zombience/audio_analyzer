using UnityEngine;
using System.Collections;

public class MoveByAmplitude : MonoBehaviour 
{
	
	
	[SerializeField]
	[Range(0, 3)]
	protected int listenBand;
	[SerializeField]
	protected float amount;
	[SerializeField]
	protected Vector3 direction;

	protected Vector3 origin;
	protected Transform trans;
	

	#region Unity Methods
	void Start ()
	{
		trans = transform;
		origin = trans.position;
	}
	
	void Update () 
	{
		trans.position = origin + (direction * AudioAnalyzer.output[listenBand] * amount);
	}
	#endregion
}
