using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AudioAnalyzer.EditorUtilities
{

	[CustomEditor(typeof(TransformFixedAFX))]
	public class TransformFixedAFXEditor : InspectorMonoBase<TransformFixedAFX>
	{

		Transform t;

		bool adjusting;

		protected override void OnEnable()
		{
			base.OnEnable();
			showDefaultInspector = true;
		}

		protected void OnDisable()
		{
			adjusting = false;
			if (t != null) GameObject.DestroyImmediate(t.gameObject);
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if(!adjusting)
			{
				editor.Button("adjust target", Color.red, 40, () =>
				{
					t = new GameObject("target").transform;
					t.position		= obj.Position;
					t.localScale	= obj.Scale;
					t.rotation		= obj.Rotation;
					adjusting		= true;
				});
			}
			else
			{
				editor.Button("save target", Color.green, 40, () =>
				{
					obj.Position	= t.position;
					obj.Scale		= t.localScale;
					obj.Rotation	= t.rotation;
					adjusting		= false;
					if (t != null) GameObject.DestroyImmediate(t.gameObject);
				});
			}
		}
	}
}
