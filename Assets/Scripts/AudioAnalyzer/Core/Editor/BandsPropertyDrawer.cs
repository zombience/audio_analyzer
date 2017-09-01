using UnityEngine;
using UnityEditor;

namespace AudioAnalyzer.EditorUtilities
{
	[CustomPropertyDrawer(typeof(BandValue), true)]
	public class BandValueDrawer : BasePropertyDrawer
	{
		bool unfold, showEase;
		
		string[] props = new string[]
		{
			"band",
			"minOutput",
			"maxOutput",
			"easeFall", // bool value
			"fallRate"	// only show if easeFall is true
		};
		

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);
			
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			Rect rect = new Rect(position.x, position.y, position.width, 10);//EditorGUI.IndentedRect(position);
			unfold = EditorGUI.Foldout(rect, unfold, "band controls");

			if (unfold)
			{
				
				// props.Length -1 is to skip fallRate, manually draw that one
				for (int i = 0; i < props.Length - 1; i++)
				{
					rect = DrawProperty(props[i], rect, property);
				}

				SerializedProperty prop = property.FindPropertyRelative("easeFall");
				showEase = prop.boolValue;
				if (showEase)
				{
					rect = DrawProperty("fallRate", rect, property);
				}
			}
			
			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label) * (unfold ?  props.Length  +  (showEase ? 2 : 0) : 1);
		}
	}
}