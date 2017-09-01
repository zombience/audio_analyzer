using UnityEngine;

namespace AudioAnalyzer
{
	public class TransformAFX : MonoBehaviour
	{

		[SerializeField] TransformMover		mover;
		[SerializeField] TransformScaler	scaler;
		[SerializeField] TransformRotator	rotator;

		#region Unity Methods
		void Start()
		{
			mover.Init(transform);
			scaler.Init(transform);
			rotator.Init(transform);
		}


		void Update()
		{
			if (mover.isActive)		mover.Update();
			if (scaler.isActive)	scaler.Update();
			if (rotator.isActive)	rotator.Update();
		}
		#endregion


		[System.Serializable]
		public abstract class TransformModule
		{

			[SerializeField]
			protected bool active;

			[SerializeField]
			protected BandValue band = new BandValue();

			/// <summary>
			/// vector to be used for either direction, scale, or axis, depending on module
			/// vector is common for editor / inspector purposes
			/// </summary>
			[SerializeField]
			protected Vector3 vector = Vector3.one;
			
			protected Transform transform;

			public bool isActive { get { return active; } }

			virtual public void Init(Transform t) { transform = t; }
			abstract public void Update();
		}

		[System.Serializable]
		class TransformMover : TransformModule
		{
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
				if (useLocalSpace) transform.localPosition = origin + (vector * band.bandValue);
				else transform.position = origin + (vector * band.bandValue);
			}
		}

		[System.Serializable]
		class TransformScaler : TransformModule
		{
			Vector3 origScale;

			public override void Init(Transform t)
			{
				base.Init(t);
				origScale = transform.localScale;
			}

			public override void Update()
			{
				transform.localScale = origScale + (vector * band.bandValue);
			}
		}

		[System.Serializable]
		class TransformRotator : TransformModule
		{
			[SerializeField] bool useLocalSpace, 
								useAdditiveRotation;

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
					transform.Rotate(vector, band.bandValue, useLocalSpace ? Space.Self : Space.World);
				}
				else
				{
					if (useLocalSpace)
					{
						transform.localRotation = origRot * Quaternion.AngleAxis(band.bandValue, vector);
					}
					else
					{
						transform.rotation		= origRot * Quaternion.AngleAxis(band.bandValue, vector);
					}
				}
			}
		}
	}
}