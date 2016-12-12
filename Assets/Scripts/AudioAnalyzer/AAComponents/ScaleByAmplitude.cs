using UnityEngine;
using System.Collections;



public class ScaleByAmplitude : AFXBase
{

    [SerializeField]
    protected Vector3 axis = Vector3.one;
    protected Vector3 originalScale;

	void Start()
    {
        originalScale = transform.localScale;
    }

	void Update ()
    {
        transform.localScale = originalScale + (axis * bandValue);
	}
}
