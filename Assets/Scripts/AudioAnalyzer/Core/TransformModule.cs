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

		virtual public void Init(Transform t) { transform = t; }
		abstract public void Update();
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

		[SerializeField]
		protected BandValueRange band = new BandValueRange();
	}

	public abstract class TransformModuleFixed : TransformModule
	{

		[SerializeField]
		protected BandValueNormalized band = new BandValueNormalized();

		[SerializeField, HideInInspector]
		protected Vector3 target = Vector3.zero;
		public Vector3 Target { get { return target; } set { target = value; } }
	}
}
