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

		protected Rect DrawProperty(string propName, Rect rect, SerializedProperty property, GUIContent label, System.Action<Rect, SerializedProperty> action = null)
		{
			var prop = property.FindPropertyRelative(propName);
			rect.y += base.GetPropertyHeight(prop, label);
			rect.height = base.GetPropertyHeight(prop, label);

			if (action == null)
			{
				EditorGUI.PropertyField(rect, prop, label);
			}
			else
			{
				action(rect, prop);
			}
			return rect;
		}
	}

	public static partial class Utilities
	{

		static public System.Action ColorUpdater { private get; set; }

		static public bool IsCurrent(this Context c)
		{
			switch (c)
			{
				case Context.PLAYING:
					return Application.isPlaying;
				case Context.NOTPLAYING:
					return !Application.isPlaying;
				case Context.EITHER:
					return true;
				default:
					return false;
			}
		}

		static public void Action(this Context c, System.Action action)
		{
			if (c.IsCurrent() && action != null) action();
		}


		static public void Button(this Context c, string label, System.Action action = null)
		{
			if (!c.IsCurrent()) return;
			if (ColorUpdater != null) ColorUpdater();

			if (GUILayout.Button(label))
				if (action != null) action();
		}

		static public void Button(this Context c, string label, Color color, float height = 40, System.Action action = null)
		{
			if (!c.IsCurrent()) return;
			Color bg = GUI.backgroundColor;
			GUI.backgroundColor = color;

			if (GUILayout.Button(label, GUILayout.Height(height)))
				if (action != null) action();

			GUI.backgroundColor = bg;
		}


		static public void Label(this Context c, string label)
		{
			if (!c.IsCurrent()) return;
			GUILayout.Label(label);
		}

		static public Texture2D BGTexture(this Color color)
		{
			var texture = new Texture2D(1, 1)
			{
				hideFlags = HideFlags.DontSave
			};
			texture.SetPixel(0, 0, color);
			texture.Apply();
			return texture;
		}

		static public void SectionLabel(this Context c, GUIStyle style, string label, Color color, int height, int width = 0)
		{
			if (!c.IsCurrent()) return;
			style.SectionLabel(label, color, height, width);
		}

		static public void SectionLabel(this GUIStyle style, string label, Color color, int height, int width = -1)
		{
			GUILayout.Space(5);
			style.normal.background = color.BGTexture();
			style.alignment = TextAnchor.UpperCenter;

			if (width < 0)
			{
				GUILayout.BeginHorizontal(style, GUILayout.Height(height));
			}
			else
			{
				GUILayout.BeginHorizontal(style, new GUILayoutOption[]
				{ GUILayout.Height(height), GUILayout.Width(width)});
			}

			GUILayout.Label(label);
			GUILayout.Space(height);
			GUILayout.EndHorizontal();
			GUILayout.Space(5);
		}
	}

}
