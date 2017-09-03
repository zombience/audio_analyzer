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
		Transform targetObj;
		Material mat;

		bool isEditing;

		protected override void OnEnable()
		{
			base.OnEnable();
			showDefaultInspector = true;
		}

		void OnDisable()
		{
			if (isEditing)
			{
				StopEdit();
			}

			Cleanup();
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

			if(Application.isPlaying && targetObj != null)
			{
				Cleanup();
			}
		}

		void StartEdit()
		{
			CreateTargetObject();

			position	= obj.transform.position;
			scale		= obj.transform.localScale;
			rotation	= obj.transform.rotation;
			
			if (obj.Position != Vector3.zero)			obj.transform.position = obj.Position;
			if (obj.Scale != Vector3.zero)				obj.transform.localScale = obj.Scale;
			if (obj.Rotation != Quaternion.identity)	obj.transform.rotation = obj.Rotation;

			obj.Position	= position;
			obj.Scale		= scale;
			obj.Rotation	= rotation;

			PlaceTargetObject();

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

			PlaceTargetObject();

			isEditing = false;
		}

		void CreateTargetObject()
		{
			if (targetObj == null)
			{
				targetObj = new GameObject("fixed_editor_helper", typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer)).transform;

				targetObj.GetComponent<MeshFilter>().mesh				= Mesh.Instantiate(obj.GetComponent<MeshFilter>().sharedMesh);
				targetObj.GetComponent<MeshCollider>().sharedMesh		= targetObj.GetComponent<MeshFilter>().sharedMesh;

				if (mat == null) mat = Instantiate(obj.GetComponent<MeshRenderer>().sharedMaterial);

				targetObj.GetComponent<MeshRenderer>().sharedMaterial		= mat;
				targetObj.GetComponent<MeshRenderer>().sharedMaterial.color	= Color.green;
				
				targetObj.parent	= obj.transform.parent;
				targetObj.hideFlags = HideFlags.HideInHierarchy;
			}
		}

		void PlaceTargetObject()
		{
			if (targetObj == null) return;
			
			targetObj.position		= obj.Position;
			targetObj.rotation		= obj.Rotation;
			targetObj.localScale	= obj.Scale;
		}

		void Cleanup()
		{
			if (targetObj != null) DestroyImmediate(targetObj.gameObject);
			GameObject leftover = GameObject.Find("fixed_editor_helper");
			if (leftover != null) DestroyImmediate(leftover);

			if (mat != null) DestroyImmediate(mat);
		}
	}
}
