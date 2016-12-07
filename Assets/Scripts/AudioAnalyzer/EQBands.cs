/* THIS CODE HAS BEEN MODIFIED FROM ITS ORIGINAL
 * WHICH CAN BE FOUND HERE: 
 * https://github.com/keijiro/Reaktion
 */


//
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

// An implementation of the state variable filter (SVF)
//
// Originally designed by H. Chamberlin and improved by P. Dutilleux.
// For further details, see the paper by B. Frei.
//
// http://courses.cs.washington.edu/courses/cse490s/11au/Readings/Digital_Sound_Generation_2.pdf

using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EQBands : MonoBehaviour
{
    [SerializeField]
    bool muteAudio = true;

    [SerializeField]
    BandPassFilter[] bands;

    // for use with custom editor: NOT for use in builds
#if UNITY_EDITOR
    public BandPassFilter[] editorBands { get { return bands; } }
#endif

    [SerializeField]
    protected AnimationCurve audioCurve = AnimationCurve.Linear(0, 0, 1, 1);

    void Awake()
    {
        BandPassFilter.audioCurve = audioCurve;
        for (int i = 0; i < bands.Length; i++)
        {
            bands[i].Init();
        }
    }



    void Update()
    {
        for (int i = 0; i < bands.Length; i++)
        {
            bands[i].UpdateBand(); 
        }
    }
        
    
    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < bands.Length; i++)
        {
            bands[i].ApplyFilter(data, channels, i == bands.Length - 1 && muteAudio); 
        }
    }

    /// <summary>
    /// poll for raw output of desired band in dB
    /// if requested band does not exist, next highest band will be returned
    /// </summary>
    /// <param name="band"></param>
    /// <returns></returns>
    public float GetRawOutput(int band)
    {
        if (band >= bands.Length) band = bands.Length - 1;
        return bands[band].level;
    }

    /// <summary>
    /// poll band and request specific scale output, e.g. 0.0-1.0
    /// </summary>
    /// <param name="band"></param>
    /// <param name="targetMin">the desired lower bound</param>
    /// <param name="band">the desired upper bound</param>
    /// <returns></returns>
    public float GetScaledOutput(int band, float targetMin, float targetMax)
    {
        if (band >= bands.Length) band = bands.Length - 1;
        return bands[band].level.Map(0, 1, targetMin, targetMax);
    }

    [System.Serializable]
    public class BandPassFilter 
    {

        //NOTE: selecting "listen" will cause the filter to be written back into the audio stream
        // this will affect all subsequent filters as long as listen is true
        [SerializeField]
        bool listen;

        //TODO: create property drawer that translates cutoff to cutOffFrequency in editor not in play mode
        [SerializeField]
        [Range(0.0f, 1.0f)]
        float cutoff = 0.5f;

        [SerializeField]
        [Range(1.0f, 30.0f)]
        float q = 1.0f; 

        [SerializeField]
        [Range(0.1f, 10.0f)]
        float bandGain = 1.0f;

        [SerializeField, Range(0.05f, 1.0f)]
        float sensitivity = .95f;

        public static AnimationCurve audioCurve;

        // Cutoff frequency in Hz
        [SerializeField]
        public float cutOffFrequency
        {
            get { return Mathf.Pow(2, 10 * cutoff - 10) * 15000; }
        }

        [SerializeField, HideInInspector]
        protected int order;

        // DSP variables
        float vF, vD, 
            vZ1, vZ2, vZ3;
    
        const float zeroOffset = 1.5849e-13f;
        const float refLevel = 0.70710678118f; // 1/sqrt(2)
        const float minDB = -60.0f;

        float squareSum, _level,
            peak, rawInput;

        [SerializeField]
        protected float headroom = 1.0f, 
            dynamicRange = 17.0f, 
            lowerBound = -60.0f,
            offset = 0;

        // for use with editor only
#if UNITY_EDITOR
        public float Peak { get { return peak; } }
        public float RawInput { get { return rawInput; } }
        public float LowerBound { get { return lowerBound; } }
        public float DynamicRange { get { return dynamicRange; } }
        public float Headroom { get { return headroom; } }
        public float Gain { get { return bandGain; } }
        public float Offset { get { return offset; } }
#endif

        int sampleCount;


        protected float dbLevel = -60.0f;

        public float level
        {
            get { return _level; }
        }

        public void Init()
        {
            peak = lowerBound + dynamicRange + headroom;
            rawInput = -1e12f;
            UpdateBand();
        }

        public void UpdateBand()
        {
            var f = 2 / 1.85f * Mathf.Sin(Mathf.PI * cutOffFrequency / AudioSettings.outputSampleRate);
            vD = 1 / q;
            vF = (1.85f - 0.75f * vD * f) * f;
            if (sampleCount < 1) return;

            var rms = Mathf.Min(1.0f, Mathf.Sqrt(squareSum / sampleCount));
            dbLevel = 20.0f * Mathf.Log10(rms / refLevel + zeroOffset);

            float input = 0.0f;

            squareSum = 0;
            sampleCount = 0;
            rawInput = dbLevel;

            peak = Mathf.Max(peak, Mathf.Max(rawInput, lowerBound + dynamicRange + headroom));

            // Normalize the input level.
            input = (rawInput - peak + headroom + dynamicRange) / dynamicRange;
            input = audioCurve.Evaluate(Mathf.Clamp01(input));

            if (sensitivity < 1.0f)
            {
                var coeff = Mathf.Pow(sensitivity, 2.3f) * -128;
                input -= (input - _level) * Mathf.Exp(coeff * Time.deltaTime);
            }
            _level = input;
        }


        public void ApplyFilter(float[] audioData, int channels, bool mute)
        {
            sampleCount += audioData.Length / channels;
            for (var i = 0; i < audioData.Length; i += channels)
            {
                var si = audioData[i];
            
                var _vZ1 = 0.5f * si;
                var _vZ3 = vZ2 * vF + vZ3;
                var _vZ2 = (_vZ1 + vZ1 - _vZ3 - vZ2 * vD) * vF + vZ2;

                squareSum += (_vZ2 * _vZ2);
                if(listen)
                {
                    for (int c = 0; c < channels; c++)
                    {
                        audioData[i + c] = _vZ2;
                    }
                }
            
                vZ1 = _vZ1;
                vZ2 = _vZ2;
                vZ3 = _vZ3;
            }
            if(mute)
            {
                for (int i = 0; i < audioData.Length; i++)
                {
                    audioData[i] = 0;
                }
            }
        }
    }

}




#if UNITY_EDITOR

[CustomEditor(typeof(EQBands))]
class EQBandsEditor : Editor
{

    SerializedProperty bands;
    SerializedProperty mute;
    SerializedProperty audioCurve;

    Color bgColor;
    
    Texture2D[] barTextures;

    EQBands.BandPassFilter[] filters;

    private void OnEnable()
    {
        bands = serializedObject.FindProperty("bands");
        mute = serializedObject.FindProperty("muteAudio");
        audioCurve = serializedObject.FindProperty("audioCurve");
        bgColor = GUI.backgroundColor;
        filters = (target as EQBands).editorBands;
    }


    void OnDisable()
    {
        if (barTextures != null)
        {
            // Destroy the bar textures.
            foreach (var texture in barTextures)
                DestroyImmediate(texture);
            barTextures = null;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(mute);
        EditorGUILayout.PropertyField(audioCurve);

        int bandCount = bands.arraySize;

        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        if(GUILayout.Button("add band", GUILayout.Width(80), GUILayout.ExpandWidth(true)))
        {
            bands.InsertArrayElementAtIndex(bands.arraySize);
            bands.GetArrayElementAtIndex(bands.arraySize - 1).FindPropertyRelative("order").intValue = bands.arraySize - 1;
        }
        GUI.backgroundColor = Color.red;
        if(GUILayout.Button("remove band", GUILayout.Width(80), GUILayout.ExpandWidth(true)))
        {
            bands.DeleteArrayElementAtIndex(bands.arraySize - 1);
        }
        GUILayout.EndHorizontal();

        GUI.backgroundColor = bgColor;
        // make sure that bands get displayed in the correct order - arrays are not guaranteed to be saved in order
        int[] order = new int[bands.arraySize];
        for (int i = 0; i < bands.arraySize; i++)
        {
            order[i] = bands.GetArrayElementAtIndex(i).FindPropertyRelative("order").intValue;
        }

        for (int i = 0; i < bands.arraySize; i++)
        {
            SerializedProperty band = bands.GetArrayElementAtIndex(order[i]);
            SerializedProperty listen = band.FindPropertyRelative("listen");
            SerializedProperty cutoff = band.FindPropertyRelative("cutoff");
            SerializedProperty q = band.FindPropertyRelative("q");
            SerializedProperty gain = band.FindPropertyRelative("bandGain");

            SerializedProperty headroom = band.FindPropertyRelative("headroom");
            SerializedProperty dynamicRange = band.FindPropertyRelative("dynamicRange");
            SerializedProperty lowerBound = band.FindPropertyRelative("lowerBound");
            SerializedProperty sensitivity = band.FindPropertyRelative("sensitivity");

            GUI.backgroundColor = Color.yellow;
            EditorGUILayout.PropertyField(listen);

            GUI.backgroundColor = Color.cyan;
            GUILayout.BeginHorizontal();
            GUILayout.Label("cutoff in hz: ", GUILayout.Width(80));
            EditorGUILayout.FloatField(Mathf.Pow(2, 10 * cutoff.floatValue - 10) * 15000);
            GUILayout.EndHorizontal();

            GUI.backgroundColor = Color.magenta;
            EditorGUILayout.PropertyField(cutoff);
            EditorGUILayout.PropertyField(q);
            EditorGUILayout.PropertyField(gain);

            EditorGUILayout.PropertyField(headroom);
            EditorGUILayout.PropertyField(dynamicRange);
            EditorGUILayout.PropertyField(lowerBound);

            GUILayout.Space(20);

            if(Application.isPlaying) DrawInputLevelBars(filters[i]);

            if (i < bands.arraySize - 1) GUILayout.Space(40);
        }
        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }

    // Make a texture which contains only one pixel.
    Texture2D NewBarTexture(Color color)
    {
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }

    // Draw the input level bar.
    void DrawInputLevelBars(EQBands.BandPassFilter band)
    {
        if (barTextures == null)
        {
            // Make textures for drawing level bars.
            barTextures = new Texture2D[] {
                NewBarTexture(new Color(55.0f / 255, 53.0f / 255, 45.0f / 255)),
                NewBarTexture(new Color(250.0f / 255, 249.0f / 255, 248.0f / 255)),
                NewBarTexture(new Color(110.0f / 255, 192.0f / 255, 91.0f / 255, 0.8f)),
                NewBarTexture(new Color(226.0f / 255, 0, 7.0f / 255, 0.8f)),
                NewBarTexture(new Color(249.0f / 255, 185.0f / 255, 22.0f / 255))
            };
        }

        // Get a rectangle as a text field and fill it.
        var rect = GUILayoutUtility.GetRect(18, 16, "TextField");
        GUI.DrawTexture(rect, barTextures[0]);

        // Draw the raw input bar.
        var temp = rect;
        temp.width *= Mathf.Clamp01((band.RawInput - band.LowerBound) / (3 - band.LowerBound));
        GUI.DrawTexture(temp, barTextures[1]);

        // Draw the dynamic range.
        temp.x += rect.width * (band.Peak - band.LowerBound - band.DynamicRange - band.Headroom) / (3 - band.LowerBound);
        temp.width = rect.width * band.DynamicRange / (3 - band.LowerBound);
        GUI.DrawTexture(temp, barTextures[2]);

        // Draw the headroom.
        temp.x += temp.width;
        temp.width = rect.width * band.Headroom / (3 - band.LowerBound);
        GUI.DrawTexture(temp, barTextures[3]);

        // Display the peak level value.
        EditorGUI.LabelField(rect, "Peak: " + band.Peak.ToString("0.0") + " dB");

        // Draw the gain level.
        DrawLevelBar("Gain", band.Gain, barTextures[0], barTextures[1]);

        // Draw the offset level.
        DrawLevelBar("Offset", band.Offset, barTextures[0], barTextures[1]);

        // Draw the output level.
        DrawLevelBar("Out", band.level, barTextures[0], barTextures[4]);
    }

    void DrawLevelBar(string label, float value, Texture bg, Texture fg)
    {
        // Get a rectangle as a text field and fill it.
        var rect = GUILayoutUtility.GetRect(18, 16, "TextField");
        GUI.DrawTexture(rect, bg);

        // Draw a level bar.
        var temp = rect;
        temp.width *= value;
        GUI.DrawTexture(temp, fg);

        // Display the level value in percentage.
        EditorGUI.LabelField(rect, label + ": " + (value * 100).ToString("0.0") + " %");
    }
}
#endif