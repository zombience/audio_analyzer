using UnityEngine;
using UnityEditor;

using System.Collections;

namespace AudioAnalyzer.EditorUtilities
{
	[CustomPropertyDrawer(typeof(TransformModuleRelative), true)]
	public class TransformAFXPropertyDrawer : BasePropertyDrawer
	{

		bool unFold;
		SerializedProperty propReference;

		string[] props = new string[]
		{
			"vector",
			"useLocalSpace",
			"useAdditiveRotation",
			"band",
		};

		float height
		{
			get
			{
				return unFold ? 11 : 2f;
			}
		}
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			
			if (propReference == null) propReference = property;

			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);
			
			int indent = EditorGUI.indentLevel;
			Rect rect = new Rect(position.x, position.y, position.width, 10);
			DrawProperty("active", new Rect(position.x, position.y - 20, position.width, 10), property, new GUIContent("Use " + property.name));

			EditorGUI.indentLevel += 2;

			rect.y += 10;
			unFold = EditorGUI.Foldout(rect, unFold, property.name); ;
			
			if(unFold)
			{
				EditorGUI.indentLevel += 2;	
				// using base.GetPropertyHeight does not seem to work for detecting whether property "band" is expanded or not
				// also, property.FindPropertyRelative("band").isExpanded is similarly failing
				// currently cannot detect whether band is expanded or not and adjust height accordingly
				// this is likely due to the fact that band has its own custom property drawer

				for (int i = 0; i < props.Length; i++)
				{
					if (property.FindPropertyRelative(props[i]) == null) continue;

					if(i == 0)
					{
						GUIContent c;
						switch (property.name)
						{
							case ("mover"):
								c = new GUIContent("Direction");
								break;
							case ("scaler"):
								c = new GUIContent("Scale");
								break;
							case ("rotator"):
								c = new GUIContent("Axis");
								break;
							default:
								c = GUIContent.none;
								break;
						}
						rect = DrawProperty(props[i], rect, property, c);
					}
					else
					{
						rect = DrawProperty(props[i], rect, property);
					}
				}
			}
			
			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label) * height;
		}

		
	}
}