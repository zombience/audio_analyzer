using UnityEngine;
using System.Collections;
using System;

public class TransformAFX : MonoBehaviour 
{
	
    
    [SerializeField]    bool useMover;
    [SerializeField]    TransformMover mover;
    [SerializeField]    bool useScale;
    [SerializeField]    TransformScaler scaler;
    [SerializeField]    bool useRotation;
    [SerializeField]    TransformRotator rotator;
    
#region Unity Methods
	void Start ()
	{
        mover.Init(transform);
        scaler.Init(transform);
        rotator.Init(transform);
	}
        
	
	void Update () 
	{
        if (useMover) mover.Update();
        if (useScale) scaler.Update();
        if (useRotation) rotator.Update();
	}
    #endregion


    [System.Serializable]
    class TransformModule
    {
        [SerializeField]
        protected int band;
        [SerializeField]
        protected float min = 0, max = 1f, offset = 0, fallRate = .3f;
        [SerializeField]
        protected bool easeFall;

        protected float bandValue
        {
            get
            {
                float newVal = AudioAnalyzer.GetScaledOutput(band, min, max);
                if (easeFall && newVal < _bandValue)
                {
                    _bandValue = Mathf.Lerp(_bandValue, newVal, Time.deltaTime * fallRate);
                }
                else
                {
                    _bandValue = newVal;
                }
                return _bandValue + offset;
            }
        }
        protected float _bandValue;

        protected Transform transform;
        virtual public void Init(Transform t) { transform = t; }
        virtual public void Update() { }
    }

    [System.Serializable]
    class TransformMover : TransformModule
    {
        [SerializeField]
        Vector3 dir = Vector3.up;
        [SerializeField]
        bool useLocalSpace;

        Vector3 origin;
        
        public override void Init(Transform t)
        {
            base.Init(t);
            if (useLocalSpace) origin = transform.localPosition;
            else origin = transform.position;
        }

        public override void Update()
        {
            if (useLocalSpace) transform.localPosition = origin + (dir * bandValue);
            else transform.position = origin + (dir * bandValue);
        }
    }

    [System.Serializable]
    class TransformScaler : TransformModule
    {

        [SerializeField]
        Vector3 scale = Vector3.one;

        Vector3 origScale;

        public override void Init(Transform t)
        {
            base.Init(t);
            origScale = transform.localScale;
        }

        public override void Update()
        {
            transform.localScale = origScale + (scale * bandValue);
        }
    }

    [System.Serializable]
    class TransformRotator : TransformModule
    {
        [SerializeField]
        Vector3 axis = Vector3.up;

        [SerializeField]
        bool useLocalSpace, useAdditiveRotation;

        Quaternion origRot;

        public override void Init(Transform t)
        {
            base.Init(t);
            origRot = useLocalSpace ? transform.localRotation : transform.rotation;
        }

        public override void Update()
        {
            if (useAdditiveRotation)
            {
                transform.Rotate(axis, bandValue, useLocalSpace ? Space.Self : Space.World);
            }
            else
            {
                if (useLocalSpace)
                {
                    transform.localRotation = origRot * Quaternion.AngleAxis(bandValue, axis);
                }
                else
                {
                    transform.rotation = origRot * Quaternion.AngleAxis(bandValue, axis);
                }
            }
        }

    }
}
