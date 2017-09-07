using UnityEngine;


namespace AudioAnalyzer
{
	public class FollowIK : AnimationAFXBase
	{

		[SerializeField]
		bool multiplyByMasterBand;

		[SerializeField]
		TargetHelper[] ikTargets;

		public float overrideamount = 1f;

		void OnAnimatorIK()
		{
			for (int i = 0; i < ikTargets.Length; i++)
			{
				TargetHelper t = ikTargets[i];

				float value = multiplyByMasterBand ? t.Value * band.bandValue : t.Value;

				if(t.isLookTarget)
				{
					anim.SetLookAtWeight(overrideamount);
					anim.SetLookAtPosition(t.Position);
				}
				else
				{
					anim.SetIKPositionWeight(t.bodyPart, overrideamount);
					anim.SetIKRotationWeight(t.bodyPart, overrideamount);
					anim.SetIKPosition(t.bodyPart, t.Position);
					anim.SetIKRotation(t.bodyPart, t.Rotation);
				}
			}
		}

		[System.Serializable]
		class TargetHelper
		{
			public BandValueNormalized band;

			public Transform	target;
			public AvatarIKGoal bodyPart;
			public bool			isLookTarget;

			public float		Value		{ get { return band.bandValue; } }
			public Vector3		Position	{ get { return target.position; } }
			public Quaternion	Rotation	{ get { return target.rotation; } }
		}
	}


}
