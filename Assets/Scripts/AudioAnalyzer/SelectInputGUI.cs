using UnityEngine;

using System;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SelectInputGUI : MonoBehaviour 
{

	protected Action<string> callback;



    void OnGUI()
	{
		float spacing = 220;
		Rect r = new Rect(100, Screen.height - 100, 200, 50);
		for (int i = 0; i < Microphone.devices.Length; i++)
		{
			if (GUI.Button(r, Microphone.devices[i]))
				callback(Microphone.devices[i]);
			r = new Rect(r.xMin + spacing, r.yMin, r.width, r.height);
		}
	}


	public void SetCallback(Action<string> cb)
	{
		callback = cb;
	}

	
}

#if UNITY_EDITOR
[CustomEditor(typeof(SelectInputGUI))]
public class SelectInputGUIEditor : Editor
{

    SelectInputGUI obj;

    void OnEnable()
    {
        obj = target as SelectInputGUI;
        if(!Application.isPlaying)
        {
            obj.hideFlags = HideFlags.HideInInspector;
        }
    }
            
    public override void OnInspectorGUI()
    {
        if(!Application.isPlaying)
        {
            Debug.LogError("SelectInputGUI should not be manually added: it will be dynamically added by AudioAnalyzer as needed\n" + 
                "default key is set to 'I', but cacn be reassigned via inspector on AudioAnalyzer script\n" +
                "SelectInputGUI is only available at runtime, selections do not currently save on stop");
            DestroyImmediate(obj);
            return;
        }
    }
}
#endif

