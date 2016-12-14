using System.Collections.Generic;
using UnityEngine;


public class RotationTest : MonoBehaviour
{

    public float speed;
    public Vector3 axis = Vector3.up;

    public bool isAdditive;

    public bool useLocalSpace;

    Quaternion origRot;

	void Start ()
    {
        origRot = transform.rotation;
	}
		
	void Update ()
    {
		if(isAdditive)
        {
            transform.Rotate(axis, speed, useLocalSpace ? Space.Self : Space.World);
        }
        else
        {
            if(useLocalSpace)
            {
                transform.localRotation = origRot * Quaternion.AngleAxis(speed, axis);
            }
            else
            {
                transform.rotation = origRot * Quaternion.AngleAxis(speed, axis);
            }
        }
	}
}
                