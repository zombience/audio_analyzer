using UnityEngine;

namespace AudioAnalyzer
{

	[System.Serializable]
	public abstract class TransformModule
	{
		[SerializeField]
		protected bool active;

		

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
		protected Vector3 target;
		public Vector3 Target { get { return target; } set { target = value; } }
	}
}
