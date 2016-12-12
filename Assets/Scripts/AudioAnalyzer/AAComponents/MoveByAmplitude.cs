using UnityEngine;
using System.Collections;
using System;

public class TransformAFX : AFXBase 
{
	[SerializeField]
	protected Vector3 moveDirection;

    [SerializeField]
    protected bool useScale, useDirection, useRotation;

    protected Vector3 origin, origScale;

    TransformModule mover, scaler, rotator;

#region Unity Methods
	void Start ()
	{
		origin = transform.position;
        origScale = transform.localScale;

        if(useScale)
        {
            mover = new TransformModule()
        }
	}
	
	void Update () 
	{
        if(useScale)
        {
            transform.localScale = origScale * bandValue;
        }

        if(useDirection)
        {
		    transform.position = origin + (moveDirection * bandValue);
        }

        if(useRotation)
        {

        }
        
	}

    [SerializeField]
    class TransformModule
    {
        protected Transform transform;
        virtual public void Init(Transform t) { transform = t; }
        virtual public void Update() { }
    }

    [SerializeField]
    class TransformMover : TransformModule
    {
        [SerializeField]
        protected float min = 0, max = 5f;
        [SerializeField]
        public Vector3 dir = Vector3.up;
        [SerializeField]
        public bool useLocalSpace;

        Vector3 origin;

        public override void Init(Transform t)
        {
            base.Init(t);
            if (useLocalSpace) origin = transform.localPosition;
            else origin = transform.position;
        }

        public override void Update(float val)
        {
            if(useLocalSpace) transform.localPosition = origin + (dir * )
        }
    }


    #endregion
}
