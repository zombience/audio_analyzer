using UnityEngine;
using System.Collections;

namespace AudioAnalyzer
{
	public class MaterialColorAFX : AFXNormalizedBase
	{
		#region vars
		[SerializeField]
		protected Color high;
		[SerializeField]
		protected Color low;

		[SerializeField]
		protected float test;

		protected Renderer rend;
		protected Material[] mats;
		#endregion

		#region Unity methods
		protected void Start()
		{
			rend = GetComponent<Renderer>();
			mats = rend.materials;
		}

		protected void Update()
		{
			for (int i = 0; i < mats.Length; i++)
			{
				mats[i].color = Color.Lerp(low, high, band.bandValue);
			}
			rend.materials = mats;
		}
		#endregion

	#if UNITY_EDITOR
		// not sure why but this was failing in custom inspector
		public void GetMatColors()
		{
			// set low and high to material color so user has the option 
			// to select which should be a custom color
			high	= GetComponent<Renderer>().sharedMaterial.color;
			low		= GetComponent<Renderer>().sharedMaterial.color;
		}
	#endif
	}
}