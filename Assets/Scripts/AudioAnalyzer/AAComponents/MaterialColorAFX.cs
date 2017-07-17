using UnityEngine;
using System.Collections;

namespace AudioAnalyzer
{
	public class MaterialColorAFX : AFXBase
	{
		#region vars
		[SerializeField]
		protected Color high;
		[SerializeField]
		protected Color low;

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
	}
}