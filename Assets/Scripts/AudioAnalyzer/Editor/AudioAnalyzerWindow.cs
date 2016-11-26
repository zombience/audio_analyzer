using UnityEngine;
using System.Collections;
using UnityEditor;

public class AudioAnalyzerWindow : EditorWindow
{

    static AudioAnalyzer obj;

    [MenuItem("Window/Audio Analyzer Window")]
    static void Init()
    {
        obj = FindObjectOfType<AudioAnalyzer>();
        if(!obj)
        {
            Debug.LogError("no audio analyzer object found in scene. this will eventually auto-add audio analyzer object, but for now add one manually");
            return;
        }

        EditorWindow window = GetWindow(typeof(AudioAnalyzerWindow), false, "Audio Meters");
        window.Show();
    }
    
	void OnGUI()
    {
        
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        EditorGUILayout.EndHorizontal();
        
        Rect bands = new Rect(0, 0, 200, 20);
        bands = GUILayoutUtility.GetLastRect();
        bands.width *= .9f;
        bands.x = 10;
        bands.y = 10;
        bands.height = 20;
        EditorGUI.ProgressBar(bands, AudioAnalyzer.GetScaledOutput(0, 10, 0, 1), "Band 0");
        bands.y += 20;
        EditorGUI.ProgressBar(bands, AudioAnalyzer.GetRawOutput(1), "Band 1");
        bands.y += 20;
        EditorGUI.ProgressBar(bands, AudioAnalyzer.GetRawOutput(2), "Band 2");
        bands.y += 20;
        EditorGUI.ProgressBar(bands, AudioAnalyzer.GetRawOutput(3), "Band 3");
        



    }
}
