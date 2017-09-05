using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
#endif
namespace AudioAnalyzer
{
	public class MaterialPropertyAFX : AFXNormalizedBase
	{

		[SerializeField]
		protected MatHelper[] properties = new MatHelper[0];


		protected Renderer rend;

		// TODO: write custom inspector that lays out available shader properties
		protected void Start()
		{
			rend = GetComponent<Renderer>();

			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i].mat == null) properties[i].mat = GetComponent<Renderer>().materials[0];
				if (!properties[i].mat.HasProperty(properties[i].property))
				{
					throw new System.Exception("material: " + properties[i].mat + " does not have property: " + properties[i].property);
				}
				for (int j = 0; j < rend.materials.Length; j++)
				{
					if (rend.materials[j].name == properties[i].mat.name) properties[i].matIxd = j;
				}
			}

		}


		protected void Update()
		{
			for (int i = 0; i < properties.Length; i++)
			{
				rend.materials[properties[i].matIxd].SetFloat(properties[i].property, band.bandValue * properties[i].inputScale);
				if (properties[i].useTextureOffset)
				{
					rend.materials[properties[i].matIxd].SetTextureOffset("_MainTex", properties[i].textureOffset * band.bandValue);
				}
			}
		}

#if UNITY_EDITOR
		public Dictionary<string, List<string>> GetShaderProperties()
		{
			Dictionary<string, List<string>> matProps = new Dictionary<string, List<string>>();
			foreach (var p in properties)
			{
				if (!matProps.ContainsKey(p.mat.name)) matProps[p.mat.name] = new List<string>();

				int c = ShaderUtil.GetPropertyCount(p.mat.shader);
				for (int i = 0; i < c; i++)
				{
					ShaderUtil.ShaderPropertyType t = ShaderUtil.GetPropertyType(p.mat.shader, i);
					if (t == ShaderUtil.ShaderPropertyType.Float || t == ShaderUtil.ShaderPropertyType.Vector || t == ShaderUtil.ShaderPropertyType.Range)
					{
						string propname = ShaderUtil.GetPropertyName(p.mat.shader, i);
						if (!matProps[p.mat.name].Contains(propname)) matProps[p.mat.name].Add(propname); // for some reason properties come out doubled
					}
				}
			}
			return matProps;
		}
#endif

		// mat helper currently only works with setting float values
		[System.Serializable]
		protected class MatHelper
		{
			// if no material is assigned, first material on renderer will be assigned
			public Material mat;
			[HideInInspector]
			public int matIxd;
			public string property;
			public float inputScale = 1f;
			public bool useTextureOffset;
			public Vector2 textureOffset { get { return new Vector2(tx.x * xScale, tx.y * yScale); } }
			[SerializeField]
			protected Vector2 tx;
			public float xScale = 1f, yScale = 1f;
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(MaterialPropertyAFX))]
	public class MaterialPropertyAFXEditor : Editor
	{

		Dictionary<string, List<string>> matProps = new Dictionary<string, List<string>>();

		void OnEnable()
		{
			MaterialPropertyAFX obj = target as MaterialPropertyAFX;
			matProps = obj.GetShaderProperties();
		}

		public override void OnInspectorGUI()
		{
			GUI.backgroundColor = Color.red;
			GUILayout.Button("Please Note: only custom properties are shown. \nBuilt in properties such as TextureOffset \nwill not be displayed", GUILayout.Height(120));
			//GUILayout.Label("built in properties such as TextureOffset will not be displayed");
			GUI.backgroundColor = Color.cyan;
			GUILayout.Button("only float and vectors are currently supported");
			GUI.backgroundColor = Color.gray;

			foreach (var k in matProps.Keys)
			{
				GUILayout.Label("material: " + k + " has following custom properties: ");
				string props = "";
				foreach (var p in matProps[k])
				{
					props += p + ", ";
				}
				GUILayout.Label(props);
			}
			EditorGUI.indentLevel = 0;

			base.OnInspectorGUI();
		}
	}
#endif
}