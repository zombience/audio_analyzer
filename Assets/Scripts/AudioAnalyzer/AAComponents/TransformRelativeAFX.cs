using UnityEngine;

namespace AudioAnalyzer
{
	public class TransformRelativeAFX : AFXRangeBase
	{
		[SerializeField] bool useMasterBand;

		[SerializeField] TransformMover		mover;
		[SerializeField] TransformScaler	scaler;
		[SerializeField] TransformRotator	rotator;

		#region Unity Methods
		void Start()
		{
			mover.Init(transform, useMasterBand);
			scaler.Init(transform, useMasterBand);
			rotator.Init(transform, useMasterBand);
		}


		void Update()
		{
			if(useMasterBand)
			{
				float value = band.bandValue;
				if (mover.isActive)
				{
					mover.Value = value;
					mover.Update();
				}

				if (scaler.isActive)
				{
					scaler.Value = value;
					scaler.Update();
				}

				if (rotator.isActive)
				{
					rotator.Value = value;
					rotator.Update();
				}
			}
			else
			{
				if (mover.isActive) mover.Update();
				if (scaler.isActive) scaler.Update();
				if (rotator.isActive) rotator.Update();
			}
		}
		#endregion
			
		#region helpers
		[System.Serializable]
		class TransformMover : TransformModuleRelative
		{
			[SerializeField]
			bool useLocalSpace;

			Vector3 origin;

			public override void Init(Transform t, bool useMaster)
			{
				base.Init(t, useMaster);
				if (useLocalSpace) origin = transform.localPosition;
				else origin = transform.position;
			}

			public override void Update()
			{
				if (useLocalSpace) transform.localPosition = origin + (vector * Value);
				else transform.position = origin + (vector * Value);
			}
		}

		[System.Serializable]
		class TransformScaler : TransformModuleRelative
		{
			Vector3 origScale;

			public override void Init(Transform t, bool useMaster)
			{
				base.Init(t, useMaster);
				origScale = transform.localScale;
			}

			public override void Update()
			{
				transform.localScale = origScale + (vector * Value);
			}

			public override void Update(float value)
			{
				transform.localScale = origScale + (vector * value);
			}
		}

		[System.Serializable]
		class TransformRotator : TransformModuleRelative
		{
			[SerializeField]
			bool	useLocalSpace, 
					useAdditiveRotation;

			Quaternion origRot;

			public override void Init(Transform t, bool useMaster)
			{
				base.Init(t, useMaster);
				origRot = useLocalSpace ? transform.localRotation : transform.rotation;
			}

			public override void Update()
			{
				if (useAdditiveRotation)
				{
					transform.Rotate(vector, Value, useLocalSpace ? Space.Self : Space.World);
				}
				else
				{
					if (useLocalSpace)
					{
						transform.localRotation = origRot * Quaternion.AngleAxis(Value, vector);
					}
					else
					{
						transform.rotation		= origRot * Quaternion.AngleAxis(Value, vector);
					}
				}
			}

			public override void Update(float value)
			{
				if (useAdditiveRotation)
				{
					transform.Rotate(vector, value, useLocalSpace ? Space.Self : Space.World);
				}
				else
				{
					if (useLocalSpace)
					{
						transform.localRotation = origRot * Quaternion.AngleAxis(value * 10f, vector);
					}
					else
					{
						transform.rotation = origRot * Quaternion.AngleAxis(value * 10f, vector);
					}
				}
			}
		}
		#endregion
	}
}