using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
#endif
namespace AudioAnalyzer
{

	/// <summary>
	/// Material Property fx are somewhat experimental and may not operate as expected
	/// </summary>
	public class MaterialPropertyAFX : AFXNormalizedBase
	{

		[SerializeField]
		protected MatHelper[] properties = new MatHelper[0];


		protected Renderer rend;
		
		protected void Start()
		{
			rend = GetComponent<Renderer>();

			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i].mat == null) continue;

				if (!properties[i].mat.HasProperty(properties[i].property))
				{
					throw new System.Exception("material: " + properties[i].mat + " does not have property: " + properties[i].property);
				}
				for (int j = 0; j < rend.sharedMaterials.Length; j++)
				{
					if (rend.sharedMaterials[j].name == properties[i].mat.name) properties[i].matIdx = j;
				}
			}

		}
		
		protected void Update()
		{
			for (int i = 0; i < properties.Length; i++)
			{
				rend.sharedMaterials[properties[i].matIdx].SetFloat(properties[i].property, band.bandValue * properties[i].inputScale);
				if (properties[i].useTextureOffset)
				{
					rend.sharedMaterials[properties[i].matIdx].SetTextureOffset("_MainTex", properties[i].textureOffset * band.bandValue);
				}
			}
		}

	}
	// mat helper currently only works with setting float values
	[System.Serializable]
	public class MatHelper
	{
		public Material mat;
		[HideInInspector]
		public int matIdx;
		public string property;
		public float inputScale = 1f;
		public bool useTextureOffset;
		public Vector2 textureOffset { get { return new Vector2(tx.x * xScale, tx.y * yScale); } }
		[SerializeField]
		protected Vector2 tx;
		public float xScale = 1f, yScale = 1f;
	}
}