using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnByAmplitude : AFXBase 
{
    
	[SerializeField]
	protected float threshold = 5f, maxObjectDistance = 10f;
	[SerializeField]
	protected int maxObjects = 30;
    
    
    protected Queue<GameObject> objectPool;

	protected void Start () 
	{
        objectPool = new Queue<GameObject>(maxObjects);
	}
	
	protected void Update()
	{
		if (bandValue > threshold)
			SpawnObject();
	}
	

	protected virtual void SpawnObject()
	{
        if(objectPool.Count >= maxObjects)
        {
            GameObject obj = objectPool.Dequeue();
            Destroy(obj);
        }
        
        GameObject newObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newObj.transform.position = transform.position + Random.onUnitSphere * Random.Range(0, maxObjectDistance);
        newObj.transform.rotation = Random.rotation;
        newObj.transform.parent = transform;
        objectPool.Enqueue(newObj);
	}
}
