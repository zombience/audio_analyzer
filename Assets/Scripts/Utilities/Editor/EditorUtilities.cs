using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace AudioAnalyzer.EditorUtilities
{
	public class BasePropertyDrawer : PropertyDrawer
	{

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			base.OnGUI(position, property, label);
		}

		protected Rect DrawProperty(string propName, Rect rect, SerializedProperty property, System.Action<Rect, SerializedProperty> action = null)
		{
			var prop	= property.FindPropertyRelative(propName);
			rect.y		+= base.GetPropertyHeight(prop, GUIContent.none);
			rect.height = base.GetPropertyHeight(prop, GUIContent.none);

			if(action == null)
			{
				EditorGUI.PropertyField(rect, prop);
			}
			else
			{
				action(rect, prop);
			}
			return rect;
		}
	}
}
