/* THIS CODE HAS BEEN MODIFIED FROM THE ORIGINAL CODE
 * ORIGINAL CODE CAN BE FOUND HERE:
 * https://github.com/keijiro/Reaktion
 */


// Reaktion - An audio reactive animation toolkit for Unity.
//
// Copyright (C) 2013, 2014 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(AudioSource))]
public class AudioInput : MonoBehaviour
{


    AudioSource audioSource;

    [SerializeField]
    bool useBakedAudio;

    [SerializeField]
    AudioClip clip;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        StartInput();
    }

    void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            audioSource.Stop();
            Microphone.End(null);
            if(!useBakedAudio) audioSource.clip = null;
        }
        else
        {
            StartInput();
        }
    }
            
    void StartInput()
    {
        var sampleRate = AudioSettings.outputSampleRate;
        if(!useBakedAudio)
        {
            // Create a clip which is assigned to the default microphone.
            audioSource.clip = Microphone.Start(null, true, 1, sampleRate);

            if (audioSource.clip != null)
            {
                // Wait until the microphone gets initialized.
                int delay = 0;
                while (delay <= 0) delay = Microphone.GetPosition(null);

                // Start playing.
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("AudioInput: Initialization failed.");
            }
        }
        else
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(AudioInput))]
public class AudioInputEditor : Editor
{
    AudioInput obj;

    SerializedProperty useBakedAudio;
    SerializedProperty clip;

    void OnEnable()
    {
        obj = target as AudioInput;
        useBakedAudio = serializedObject.FindProperty("useBakedAudio");
        clip = serializedObject.FindProperty("clip");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(useBakedAudio, new GUIContent("use baked audio"));
        EditorGUILayout.PropertyField(clip, new GUIContent("audio clip"));
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

