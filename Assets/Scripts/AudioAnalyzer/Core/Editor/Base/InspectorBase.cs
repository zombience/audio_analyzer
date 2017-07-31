using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace AudioAnalyzer.EditorUtilities
{
	public class ScriptableObjectInspectorBase<T> : Editor where T : ScriptableObject
	{
		protected T obj;
		protected GUIStyle style;
		protected float inc		= .1f;
		protected Color bg		= Color.cyan;

		protected virtual void OnEnable()
		{
			obj = target as T;
			style = new GUIStyle();
			GUIUtilities.ColorUpdater = UpdateColors;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			bg = Color.cyan;
		}


		protected void Space(float pixels)
		{
			GUILayout.Space(pixels);
		}

		void UpdateColors()
		{
			GUI.backgroundColor = bg;
			bg.g -= inc;
			bg.r += inc;
			if (bg.g <= 0 || bg.g > 1f) inc *= -1f;
		}
	}
	
	public class InspectorBase<T> : Editor where T : MonoBehaviour
	{

		protected T obj;
		protected float inc = .1f;
		protected GUIStyle style;
		protected Color bg = Color.cyan;
		protected bool showDefaultInspector = true;

		protected Context 
			play	= Context.PLAYING,
			editor	= Context.NOTPLAYING, 
			either	= Context.EITHER;

		protected virtual void OnEnable()
		{
			obj = target as T;
			style = new GUIStyle();
			GUIUtilities.ColorUpdater = UpdateColors;
		}

		public override void OnInspectorGUI()
		{
			if (showDefaultInspector)
			{
				base.OnInspectorGUI();
			}
			bg = Color.cyan;
		}


		protected void Space(float pixels)
		{
			GUILayout.Space(pixels);
		}

		void UpdateColors()
		{
			GUI.backgroundColor = bg;
			bg.g -= inc;
			bg.r += inc;
			if (bg.g <= 0 || bg.g > 1f) inc *= -1f;
		}
	}

	public enum Context
	{
		PLAYING,
		NOTPLAYING,
		EITHER
	}

	public static partial class GUIUtilities
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
			if (!c.IsCurrent()) return;
			if (action != null) action();
		}

		static public void Button(this Context c, string label, System.Action action = null)
		{
			if (!c.IsCurrent())			return;
			if(ColorUpdater != null)	ColorUpdater();

			if (GUILayout.Button(label) && action != null)  action();
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

		static public void SectionLabel(this GUIStyle style, string label, int height, Color color)
		{
			GUILayout.Space(10);
			style.normal.background = color.BGTexture();
			GUILayout.BeginHorizontal(style, GUILayout.Height(height));
			GUILayout.Label(label);
			GUILayout.Space(height);
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
		}
	}
}