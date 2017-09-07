using UnityEngine;
using UnityEditor;
using System.Linq;

namespace AudioAnalyzer.EditorUtilities
{
	[CustomEditor(typeof(TransformRelativeAFX))]
	public class TransformRelativeAFXEditor : InspectorMonoBase<TransformRelativeAFX>
	{

		SerializedProperty
			unfoldMover,
			unfoldScaler,
			unfoldRotator;
			

		SerializedProperty
			mover,
			scaler,
			rotator;

		protected override void OnEnable()
		{
			base.OnEnable();

			mover	= serializedObject.FindProperty("mover");
			scaler	= serializedObject.FindProperty("scaler");
			rotator = serializedObject.FindProperty("rotator");
		}

		public override void OnInspectorGUI()
		{
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