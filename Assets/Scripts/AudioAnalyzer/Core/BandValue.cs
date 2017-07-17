using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using AudioAnalyzer;

namespace AudioAnalyzer
{
	[System.Serializable]
	public class BandValue
	{
		// choose which audio band to drive interactions
		[SerializeField]	protected int band;

		[SerializeField]	protected bool easeFall;

		// set the range of min/max to achieve desired range of effect
		[SerializeField]
		protected float
			fallRate = 1f,
			minOutput = 0f,
			maxOutput = 1f,
			offset = 0f;

		public float bandValue
		{
			get
			{
				float newVal = AudioAnalyzer.GetScaledOutput(band, minOutput, maxOutput);
				if (easeFall && newVal < _bandValue)
				{
					_bandValue = Mathf.Lerp(_bandValue, newVal, Time.deltaTime * fallRate);
				}
				else
				{
					_bandValue = newVal;
				}
				return _bandValue + offset;
			}
		}

		protected float _bandValue;
	}
}

//#if UNITY_EDITOR
//[CustomPropertyDrawer(typeof(AFXBase))]
//class AFXBaseDrawer : PropertyDrawer
//{

//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        return base.GetPropertyHeight(property, label);
//    }

//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        base.OnGUI(position, property, label);
//    }

//}

//[CustomEditor(typeof(AFXBase))]
//public class AFXBaseEditor : Editor 
//{
//    SerializedProperty band;
//    SerializedProperty easeFall;
//    SerializedProperty fallRate;
//    SerializedProperty minOutput;
//    SerializedProperty maxOutput;
    

//    protected virtual void OnEnable()
//    {
//        band = serializedObject.FindProperty("band");
//        easeFall = serializedObject.FindProperty("easeFall");
//        fallRate = serializedObject.FindProperty("fallRate");
//        minOutput = serializedObject.FindProperty("minOutput");
//        maxOutput = serializedObject.FindProperty("maxOutput");
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();



//        EditorGUILayout.PropertyField(band, GUILayout.Width(40));

        
//        GUILayout.BeginHorizontal();
//        EditorGUILayout.PropertyField(easeFall, GUILayout.Width(40), GUILayout.ExpandWidth(false) );

//        if(easeFall.boolValue)
//        {
//            GUILayout.Label("", GUILayout.Width(80));
//            EditorGUILayout.PropertyField(fallRate, GUILayout.Width(40), GUILayout.ExpandWidth(false));
//        }
//        GUILayout.EndHorizontal();

//        GUILayout.BeginHorizontal();
//        EditorGUILayout.PropertyField(minOutput, GUILayout.Width(40), GUILayout.ExpandWidth(false));
//        GUILayout.Label("", GUILayout.Width(80));
//        EditorGUILayout.PropertyField(maxOutput, GUILayout.Width(40), GUILayout.ExpandWidth(false));
//        GUILayout.EndHorizontal();

//        serializedObject.ApplyModifiedProperties();
//    }
//}
//#endif




