using UnityEngine;
using UnityEditor;
using System.Linq;

namespace AudioAnalyzer.EditorUtilities
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(TransformRelativeAFX))]
	public class TransformRelativeAFXEditor : InspectorMonoBase<TransformRelativeAFX>
	{

		SerializedProperty
			unFoldMaster,
			unfoldMover,
			unfoldScaler,
			unfoldRotator;
		
		SerializedProperty
			mover,
			scaler,
			rotator;

		SerializedProperty
			masterBand;

		protected override void OnEnable()
		{
			base.OnEnable();

			mover	= serializedObject.FindProperty("mover");
			scaler	= serializedObject.FindProperty("scaler");
			rotator = serializedObject.FindProperty("rotator");

			masterBand		= serializedObject.FindProperty("band");
			unFoldMaster	= serializedObject.FindProperty("useMasterBand");
		}

		public override void OnInspectorGUI()
		{
			style.SectionLabel("Master Band", Color.blue * .5f, 20);

			unFoldMaster.boolValue = EditorGUILayout.Foldout(unFoldMaster.boolValue, new GUIContent("Using " + (unFoldMaster.boolValue ? "Master " : "Individual ") + "Band Settings"));
			int indent = EditorGUI.indentLevel;
			if (unFoldMaster.boolValue)
			{
				EditorGUI.indentLevel += 1;
				EditorGUILayout.PropertyField(masterBand);
			}
			EditorGUI.indentLevel = indent;

			style.SectionLabel("Modules", Color.blue * .5f, 20);
			DisplayTransformModule(mover);
			DisplayTransformModule(scaler);
			DisplayTransformModule(rotator);

			serializedObject.ApplyModifiedProperties();
		}

		void DisplayTransformModule(SerializedProperty module)
		{

			SerializedProperty isActive = module.FindPropertyRelative("active");
			SerializedProperty unfold	= module.FindPropertyRelative("unfold");
			GUILayout.BeginHorizontal();

			unfold.boolValue = EditorGUILayout.Foldout(unfold.boolValue, new GUIContent(module.name));
			EditorGUILayout.PropertyField(isActive, new GUIContent("Enable " + module.name));
			GUILayout.EndHorizontal();

			int indent = EditorGUI.indentLevel;

			if(unfold.boolValue)
			{
				EditorGUI.indentLevel += 1;
				if (module.name == "mover")			DrawMover(module);
				else if (module.name == "scaler")	DrawScaler(module);
				else if (module.name == "rotator")	DrawRotator(module);
				
				SerializedProperty band	= module.FindPropertyRelative("band");
				EditorGUI.indentLevel += 1;
				EditorGUILayout.PropertyField(band);

			}
			
			EditorGUI.indentLevel = indent;
		}

		void DrawMover(SerializedProperty module)
		{
			SerializedProperty vector	= module.FindPropertyRelative("vector");
			SerializedProperty local	= module.FindPropertyRelative("useLocalSpace");

			EditorGUILayout.PropertyField(vector, new GUIContent("Direction"));
			EditorGUILayout.PropertyField(local);

		}

		void DrawScaler(SerializedProperty module)
		{
			SerializedProperty vector = module.FindPropertyRelative("vector");
			EditorGUILayout.PropertyField(vector, new GUIContent("Scale"));
		}

		void DrawRotator(SerializedProperty module)
		{
			SerializedProperty vector	= module.FindPropertyRelative("vector");
			SerializedProperty local	= module.FindPropertyRelative("useLocalSpace");
			SerializedProperty add		= module.FindPropertyRelative("useAdditiveRotation");

			EditorGUILayout.PropertyField(vector, new GUIContent("Axis"));
			EditorGUILayout.PropertyField(local);
			EditorGUILayout.PropertyField(add);
		}
	}
}