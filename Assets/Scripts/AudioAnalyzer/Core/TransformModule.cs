using UnityEngine;

namespace AudioAnalyzer
{

	// I initially did work to abstract both Relative and Fixed transform modules 
	// so that they could both use the same property drawer
	// but turns out the Fixed modules do not need extra foldouts
	// so it is now handled in TransformFixedAFXEditor
	// but now that this is here I'm not going to undo it. It may be useful for something else

	[System.Serializable]
	public abstract class TransformModule
	{
		[SerializeField]
		protected bool active;

#if UNITY_EDITOR
		// for use with inspector, so that expansion sticks after play or unfocus object
		[SerializeField, HideInInspector]
		protected bool unfold;
#endif

		protected Transform transform;

		public bool isActive { get { return active; } }

		abstract public void Init(Transform t, bool useMasterBand);
		abstract public void Update();

		/// for use when master band is in use rather than each individual module 
		/// calculating band values
		/// </summary>
		/// <param name="value"></param>
		public virtual void Update(float value) { }
	}


	[System.Serializable]
	public abstract class TransformModuleRelative : TransformModule
	{
		/// <summary>
		/// vector to be used for either direction, scale, or axis, depending on module
		/// vector is common for editor / inspector purposes
		/// </summary>
		[SerializeField]
		protected Vector3 vector = Vector3.one;


		public float Value { get { return UseMasterBand ? value : band.bandValue; } set { this.value = value; } }
		protected float value;
		/// <summary>
		/// for use when master band is in use rather than each individual module 
		/// calculating band values
		/// </summary>
		protected bool UseMasterBand { get; set; }


		[SerializeField]
		protected BandValueRange band = new BandValueRange();


		public override void Init(Transform t, bool useMasterBand)
		{
			transform = t;
			UseMasterBand = useMasterBand;
		}
	}

	public abstract class TransformModuleFixed : TransformModule
	{

		[SerializeField]
		protected BandValueNormalized band = new BandValueNormalized();

		public float Value { get { return UseMasterBand ? value : band.bandValue; } set { this.value = value; } }
		protected float value;
		/// <summary>
		/// for use when master band is in use rather than each individual module 
		/// calculating band values
		/// </summary>
		protected bool UseMasterBand { get; set; }

		[SerializeField, HideInInspector]
		protected Vector3 target = Vector3.zero;
		public Vector3 Target { get { return target; } set { target = value; } }

		public override void Init(Transform t, bool useMasterBand)
		{
			transform = t;
			UseMasterBand = useMasterBand;
		}
	}
}
