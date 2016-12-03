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

        EditorWindow window = GetWindow(typeof(AudioAnalyzerWindow), false, "Audio Analyzer Debugs");
        window.Show();
    }
    
	void OnGUI()
    {
        
        


    }
}
