using UnityEngine;
using System.Collections;



public class WanderByAmplitude : AFXBase
{

    [SerializeField]
    protected float headingChangeSpeed = .5f, 
        maxWanderRange = 5f;

    protected Vector3 origin, dir;

	void Start ()
    {
        origin = transform.position;
        dir = transform.forward;
	}
	
	void Update ()
    {

        Vector3 newDir = Random.onUnitSphere;

        dir = Vector3.Lerp(dir, newDir, headingChangeSpeed * bandValue);
        dir *= bandValue;

        if(Vector3.Distance(transform.position + dir, origin) > maxWanderRange)
        {
            dir = (origin - transform.position).normalized;
        }

        transform.position += dir;

	}
}
