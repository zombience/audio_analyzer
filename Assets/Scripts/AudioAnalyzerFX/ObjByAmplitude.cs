using UnityEngine;
using System.Collections;

public class ObjByAmplitude<T> : MonoBehaviour 
{

	protected T objPrefab;

	[Range(0, 3)]	 
	[SerializeField]
	protected int listenBand;
	[SerializeField]
	protected float threshold;
	[SerializeField]
	protected int maxObjects;

	protected Transform trans;

	protected void Start () 
	{
		trans = transform;
		StartCoroutine(ThresholdCheck());
		CreatePool();
	}

	protected IEnumerator ThresholdCheck()
	{
		yield return new WaitForSeconds(5f); // wait for mic to come online
		while (true)
		{
			float val = AudioAnalyzer.output[listenBand];
			if (val > threshold)
				SpawnObject();
			yield return null;
		}
	}
	
	protected virtual void CreatePool()
	{
		// create the object pool
	}

	protected virtual void SpawnObject()
	{
		// spawn object in child
	}
}
