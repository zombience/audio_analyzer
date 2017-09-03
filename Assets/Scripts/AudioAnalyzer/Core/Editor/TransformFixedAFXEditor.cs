using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AudioAnalyzer.EditorUtilities
{
	[CustomEditor(typeof(TransformFixedAFX))]
	public class TransformFixedAFXEditor : InspectorMonoBase<TransformFixedAFX>
	{

		Vector3 position, scale;
		Quaternion rotation;
		

		bool isEditing;

		protected override void OnEnable()
		{
			base.OnEnable();
			showDefaultInspector = true;
		}

		protected void OnDisable()
		{
			if (isEditing)
			{
				StopEdit();
			}
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if(isEditing)
			{
				style.SectionLabel("adjust target position / scale / rotation", 40, Color.cyan);
				editor.Button("save target", Color.green, 40, StopEdit);
			}
			else
			{
				editor.Button("adjust target", Color.red, 40, StartEdit);
			}
		}

		void StartEdit()
		{
			position	= obj.transform.position;
			scale		= obj.transform.localScale;
			rotation	= obj.transform.rotation;

			if (obj.Position != Vector3.zero)			obj.transform.position = obj.Position;
			if (obj.Scale != Vector3.zero)				obj.transform.localScale = obj.Scale;
			if (obj.Rotation != Quaternion.identity)	obj.transform.rotation = obj.Rotation;

			obj.Position	= position;
			obj.Scale		= scale;
			obj.Rotation	= rotation;

			isEditing = true;
		}

		void StopEdit()
		{
			obj.Position	= obj.transform.position;
			obj.Scale		= obj.transform.localScale;
			obj.Rotation	= obj.transform.rotation;

			obj.transform.position		= position;
			obj.transform.localScale	= scale;
			obj.transform.rotation		= rotation;

			isEditing = false;
		}
	}
}
