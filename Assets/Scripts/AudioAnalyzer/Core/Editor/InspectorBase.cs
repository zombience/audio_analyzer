using UnityEngine;
using UnityEditor;


namespace AudioAnalyzer.EditorUtilities
{

	abstract public class InspectorBase : Editor
	{
		protected GUIStyle	style;
		protected float		inc	= .1f;
		protected Color		bg	= Color.cyan;
		protected bool		showDefaultInspector = true;

		protected Context play		= Context.PLAYING;
		protected Context editor	= Context.NOTPLAYING;
		protected Context either	= Context.EITHER;

		protected virtual void OnEnable()
		{
			style = new GUIStyle();
			Utilities.ColorUpdater = UpdateColors;
		}

		public override void OnInspectorGUI()
		{
			if (showDefaultInspector)
			{
				base.OnInspectorGUI();
			}
			bg = Color.cyan;
		}

		protected void Space(float height)
		{
			GUILayout.Space(height);
		}

		void UpdateColors()
		{
			GUI.backgroundColor = bg;
			bg.g -= inc;
			bg.r += inc;
			if (bg.g <= 0 || bg.g > 1f) inc *= -1f;
		}
	}

	abstract public class InspectorScriptableObjectBase<T> : InspectorBase where T : ScriptableObject
	{
		protected T obj;

		protected override void OnEnable()
		{
			obj = target as T;
			base.OnEnable();
		}
	}

	/// <summary>
	/// because this is used to generate custom inspectors and I like creating custom inspectors
	/// inside the same script as monobehaviours, this class cannot be inside an Editor Folder
	/// otherwise they cannot be referenced from Monobehaviour scripts
	/// </summary>
	/// <typeparam name="T"></typeparam>
	abstract public class InspectorMonoBase<T> : InspectorBase where T : MonoBehaviour
	{
		protected T obj;

		protected override void OnEnable()
		{
			obj = target as T;
			base.OnEnable();
		}
	}

	public enum Context
	{
		PLAYING,
		NOTPLAYING,
		EITHER
	}
}