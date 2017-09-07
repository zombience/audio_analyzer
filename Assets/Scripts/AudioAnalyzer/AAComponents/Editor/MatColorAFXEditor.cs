using UnityEngine;
using UnityEditor;


namespace AudioAnalyzer.EditorUtilities
{
	[CustomEditor(typeof(MaterialColorAFX))]
	public class MatColorAFXEditor : InspectorMonoBase<MaterialColorAFX>
	{
		bool hasAssignedColors;
		
		protected override void OnEnable()
		{
			base.OnEnable();
			showDefaultInspector = true;	

			if(serializedObject.FindProperty("low").colorValue == Color.black && serializedObject.FindProperty("high").colorValue == Color.black)
			{
				obj.GetMatColors();
			}
		}
	}
}