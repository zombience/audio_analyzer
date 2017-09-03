using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioAnalyzer
{
	public class TransformFixedAFX : MonoBehaviour
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
		class TransformMover : TransformModuleFixed
		{
			Vector3 origin;

			public override void Init(Transform t)
			{
				base.Init(t);

				origin = transform.position;
			}

			public override void Update()
			{
				transform.position = Vector3.Lerp(origin, Target, band.bandValue);
			}
		}

		[System.Serializable]
		class TransformScaler : TransformModuleFixed
		{
			Vector3 origScale;

			public override void Init(Transform t)
			{
				base.Init(t);
				origScale = transform.localScale;
			}

			public override void Update()
			{
				transform.localScale = Vector3.Lerp(origScale, Target, band.bandValue);
			}
		}

		[System.Serializable]
		class TransformRotator : TransformModuleFixed
		{
			Quaternion origRot;
			[SerializeField, HideInInspector]
			Quaternion targetRot;

			public Quaternion TargetRotation { set { targetRot = value; } get { return targetRot; } }

			public override void Init(Transform t)
			{
				base.Init(t);
				origRot = transform.rotation;
			}

			public override void Update()
			{
				transform.rotation = Quaternion.Lerp(origRot, targetRot, band.bandValue);
			}
		}

#if UNITY_EDITOR
		[HideInInspector]
		public bool isEditing;

		public Vector3 Position
		{
			get { return mover.Target; }
			set { mover.Target = value; }
		}
		public Vector3 Scale
		{
			get { return scaler.Target; }
			set { scaler.Target = value; }
		}
		public Quaternion Rotation
		{
			get { return rotator.TargetRotation; }
			set { rotator.TargetRotation = value; }
		}
		
		private void OnDrawGizmosSelected()
		{
			Debug.DrawLine(transform.position, Position, Color.red);
		}
#endif
	}
}
