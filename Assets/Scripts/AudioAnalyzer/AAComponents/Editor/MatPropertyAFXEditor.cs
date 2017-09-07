using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace AudioAnalyzer.EditorUtilities
{

	[CustomEditor(typeof(MaterialPropertyAFX), true)]
	public class MatPropertyAFXEditor : InspectorMonoBase<MaterialPropertyAFX>
	{
		Dictionary<Material, List<string>> matProps = new Dictionary<Material, List<string>>();

		bool[] matFoldout, propFoldout;
		Material[] mats;
		

		SerializedProperty properties;


		protected override void OnEnable()
		{
			base.OnEnable();

			if (obj.GetComponent<Renderer>() == null) return;
			
			mats = obj.GetComponent<Renderer>().sharedMaterials;
			PopulateShaderProps();
			properties = serializedObject.FindProperty("properties");

			matFoldout = new bool[mats.Length];
			propFoldout = new bool[properties.arraySize];
		}

		public override void OnInspectorGUI()
		{
			CleanupRemovedMaterials();
			Space(10);

			GUI.backgroundColor = Color.grey;

			style.SectionLabel("Band ", Color.cyan * .6f, 20);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("band"));

			if(mats == null)
			{
				style.SectionLabel("no materials found on object", Color.red * .4f, 40);
				return;
			}
			int indent = EditorGUI.indentLevel;
			
			style.SectionLabel("materials ", Color.blue* .4f, 20);
			for (int i = 0; i < mats.Length; i++)
			{
				editor.Action(() =>
				{
					EditorGUILayout.BeginHorizontal();

					EditorGUI.indentLevel += 2;
					matFoldout[i] = EditorGUILayout.Foldout(matFoldout[i], new GUIContent((matFoldout[i] ? "hide" : "show") + " properties"));
					EditorGUILayout.ObjectField(mats[i], typeof(Material), true);

					EditorGUILayout.EndHorizontal();
				});
				

				if(matFoldout[i])
				{
					EditorGUI.indentLevel += 2;
					style.SectionLabel(mats[i].name + " properties:", Color.blue * .4f, 10);
					foreach(var propName in matProps[mats[i]])
					{
						GUILayout.BeginHorizontal(style);

						GUILayout.Label(propName, GUILayout.Width(120));

						if (!IsAlreadyAssigned(mats[i], propName))
						{
							GUI.backgroundColor = Color.green;
							if(GUILayout.Button("add prop to fx list"))
							{
								AssignProperty(mats[i], propName, i);
							}
							GUI.backgroundColor = Color.grey;
						}
						else
						{
							GUI.backgroundColor = Color.red;
							if(GUILayout.Button("remove property"))
							{
								int removeIdx = -1;
								for(int j = 0; j < properties.arraySize; j++)
								{
									if(properties.GetArrayElementAtIndex(j).FindPropertyRelative("property").stringValue == propName)
									{
										removeIdx = j;
										break;
									}
								}
								properties.DeleteArrayElementAtIndex(removeIdx);
								break;
							}
							GUI.backgroundColor = Color.grey;
						}

						GUILayout.EndHorizontal();
					}
				}
				EditorGUI.indentLevel = indent;
			}

			Space(30);

			style.SectionLabel("Active Properties", Color.magenta * .4f, 20);
			
			for (int i = 0; i < properties.arraySize; i++)
			{
				DrawMatHelper(properties.GetArrayElementAtIndex(i), i);
			}

			EditorGUI.indentLevel = indent;
			serializedObject.ApplyModifiedProperties();
		}


		void DrawMatHelper(SerializedProperty helper, int idx)
		{
			int indent = EditorGUI.indentLevel;
			GUILayout.BeginHorizontal();

			EditorGUILayout.LabelField("Material", GUILayout.Width(50));
			EditorGUILayout.PropertyField(helper.FindPropertyRelative("mat"), GUIContent.none, GUILayout.Width(140));
			EditorGUILayout.SelectableLabel(helper.FindPropertyRelative("property").stringValue);
			
			GUILayout.EndHorizontal();
			
			EditorGUILayout.PropertyField(helper.FindPropertyRelative("useInstanceMat"), new GUIContent("Use Instance Material"));

			GUILayout.BeginHorizontal();
			EditorGUI.indentLevel += 1;


			EditorGUILayout.LabelField("Input Scale", GUILayout.Width(100));
			EditorGUILayout.PropertyField(helper.FindPropertyRelative("inputScale"), GUIContent.none, GUILayout.Width(40));

			GUILayout.EndHorizontal();
			
			var prop = helper.FindPropertyRelative("useTextureOffset");
			prop.boolValue = EditorGUILayout.Foldout(prop.boolValue, new GUIContent((prop.boolValue ? "Disable" : "Enable") + " Texture Offset"));

			if (prop.boolValue)
			{
				EditorGUI.indentLevel += 1;
				EditorGUILayout.PropertyField(helper.FindPropertyRelative("tx"), new GUIContent("Base Offset"));

				SerializedProperty x = helper.FindPropertyRelative("xScale");
				SerializedProperty y = helper.FindPropertyRelative("yScale");

				Vector2 vec = new Vector2(x.floatValue, y.floatValue);
				vec = EditorGUILayout.Vector2Field("Scale", vec);

				x.floatValue = vec.x;
				y.floatValue = vec.y;
				
			}


			EditorGUI.indentLevel = indent;
			Space(10);
		}

		bool IsAlreadyAssigned(Material mat, string propName)
		{
			if (properties == null || properties.arraySize < 1) return false;
			for (int i = 0; i < properties.arraySize; i++)
			{
				SerializedProperty prop = properties.GetArrayElementAtIndex(i);
				if (prop == null)
					continue;

				SerializedProperty subProp = prop.FindPropertyRelative("mat");

				if (subProp == null || subProp.objectReferenceValue != mat)
					continue;

				subProp = prop.FindPropertyRelative("property");

				if (subProp == null || subProp.stringValue != propName)
					continue;

				return true;

			}
			return false;
		}

		void AssignProperty(Material mat, string propName, int idx)
		{
			properties.InsertArrayElementAtIndex(properties.arraySize);
			SerializedProperty prop = properties.GetArrayElementAtIndex(properties.arraySize - 1);

			prop.FindPropertyRelative("mat").objectReferenceValue	= mat;
			prop.FindPropertyRelative("property").stringValue		= propName;
			prop.FindPropertyRelative("matIdx").intValue			= idx;
			prop.FindPropertyRelative("inputScale").floatValue		= 1f;
			prop.FindPropertyRelative("useInstanceMat").boolValue	= true;
		}


		void PopulateShaderProps()
		{
			for (int i = 0; i < mats.Length; i++)
			{
				Material mat = mats[i];
				if (!matProps.ContainsKey(mats[i])) matProps[mat] = new List<string>();
				for (int j = 0; j < ShaderUtil.GetPropertyCount(mat.shader); j++)
				{
					ShaderUtil.ShaderPropertyType t = ShaderUtil.GetPropertyType(mat.shader, j);
					// || t == ShaderUtil.ShaderPropertyType.Vector 
					if (t == ShaderUtil.ShaderPropertyType.Float || t == ShaderUtil.ShaderPropertyType.Range)
					{
						string propname = ShaderUtil.GetPropertyName(mat.shader, j);
						if (!matProps[mat].Contains(propname)) matProps[mat].Add(propname); // properties come out doubled
					}
				}
			}
		}

		void CleanupRemovedMaterials()
		{

			if (Application.isPlaying) return;

			int idx = -1;
			for (int i = 0; i < properties.arraySize; i++)
			{
				SerializedProperty cur = properties.GetArrayElementAtIndex(i).FindPropertyRelative("mat");
				for (int j = 0; j < mats.Length; j++)
				{

					if(!mats.Contains(cur.objectReferenceValue))
					{
						idx = i;
						break;
					}
				}
			}
			if(idx >= 0)
			{
				properties.DeleteArrayElementAtIndex(idx);
			}
		}
	}
}