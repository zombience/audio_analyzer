using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AudioAnalyzer
{
	/// <summary>
	/// SpawnAFX will instantiate objects while band input is over threshold
	/// in order to create your own spawn logic, create a new class and inherit from SpawnAFX
	/// and override SpawnObject
	/// </summary>
	public class SpawnAFX : AFXNormalizedBase
	{

		[SerializeField]
		protected int	maxObjects	= 10;
		[SerializeField, Range(0.1f,0.95f)]
		protected float threshold	= 0.5f;


		protected Queue<GameObject> objQueue;

		protected void Start()
		{
			objQueue = new Queue<GameObject>(maxObjects);
		}

		protected void Update()
		{
			if (band.bandValue > threshold) SpawnObject();
		}

		
		protected virtual void SpawnObject()
		{
			if (objQueue.Count >= maxObjects)
			{
				GameObject obj = objQueue.Dequeue();
				Destroy(obj);
			}

			GameObject newObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			Vector3 pos = Random.onUnitSphere * 1.5f;
			newObj.transform.parent = transform;
			newObj.transform.localPosition = pos;
			newObj.transform.localScale = Vector3.one * .3f;

			Rigidbody body = newObj.AddComponent<Rigidbody>();
			body.AddForce(pos + Vector3.up * 3f, ForceMode.VelocityChange);
			objQueue.Enqueue(newObj);
		}
	}
}