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

		SerializedProperty
			unfoldMover, 
			unfoldScaler, 
			unfoldRotator;

		SerializedProperty
			mover,
			scaler,
			rotator;

		bool isEditing;

		protected override void OnEnable()
		{
			base.OnEnable();
			showDefaultInspector = true;

			mover	= serializedObject.FindProperty("mover");
			scaler	= serializedObject.FindProperty("scaler");
			rotator = serializedObject.FindProperty("rotator");
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
			DisplayTransformModule(mover);
			DisplayTransformModule(scaler);
			DisplayTransformModule(rotator);

			if(isEditing)
			{
				style.SectionLabel("Adjust target position / scale / rotation \nin Scene Window", 40, Color.black);
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

			serializedObject.ApplyModifiedProperties();
		}

		void DisplayTransformModule(SerializedProperty module)
		{

			SerializedProperty isActive = module.FindPropertyRelative("active");
			SerializedProperty unfold	= module.FindPropertyRelative("unfold");
			SerializedProperty band		= module.FindPropertyRelative("band");

			SerializedProperty bandIdx	= band.FindPropertyRelative("band");
			SerializedProperty easeFall = band.FindPropertyRelative("easeFall");
			SerializedProperty fallRate = band.FindPropertyRelative("fallRate");

			
			GUILayout.BeginHorizontal();

			unfold.boolValue = EditorGUILayout.Foldout(unfold.boolValue, new GUIContent(module.name));
			EditorGUILayout.PropertyField(isActive, new GUIContent("Enable " + module.name));

			GUILayout.EndHorizontal();

			int indent = EditorGUI.indentLevel;
			if(unfold.boolValue)
			{
				EditorGUI.indentLevel += 1;

				EditorGUILayout.PropertyField(bandIdx, new GUIContent("Band Index"));
				EditorGUILayout.PropertyField(easeFall, new GUIContent("Ease Fall Rate"));
				
				if(easeFall.boolValue)
				{
					EditorGUI.indentLevel += 1;

					EditorGUILayout.PropertyField(fallRate, new GUIContent("Fall Rate"));
				}
			}
			EditorGUI.indentLevel = indent;
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
