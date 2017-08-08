using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AudioAnalyzer;
using AudioAnalyzer.EditorUtilities;
using AudioAnalyzer.Inspector;

[CustomEditor(typeof(TestObject))]
public class TestInspector : EQBandsEditor
{

	protected override void OnEnable()
	{
		FilterBands bands = FindObjectOfType<AudioAnalyzer.AudioAnalyzer>().GetComponent<FilterBands>();
		filters = bands.editorBands;
		
		base.OnEnable();
	}
}
