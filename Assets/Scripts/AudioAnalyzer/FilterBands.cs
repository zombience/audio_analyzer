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
using UnityEngine;
using System.Collections;

// An implementation of the state variable filter (SVF)
//
// Originally designed by H. Chamberlin and improved by P. Dutilleux.
// For further details, see the paper by B. Frei.
//
// http://courses.cs.washington.edu/courses/cse490s/11au/Readings/Digital_Sound_Generation_2.pdf


public class FilterBands : MonoBehaviour
{
    [SerializeField]
    public BandPassFilter band;

    void Awake()
    {
        Update();
    }

    void Update()
    {
        
        band.UpdateBand();
        
    }
    
    void OnAudioFilterRead(float[] data, int channels)
    {
        data = band.ApplyFilter(data, channels);
    }
}



[System.Serializable]
public class BandPassFilter
{
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float cutoff = 0.5f;

    [SerializeField]
    [Range(1.0f, 10.0f)]
    float q = 1.0f;

    // Cutoff frequency in Hz
    float cutOffFrequency
    {
        get { return Mathf.Pow(2, 10 * cutoff - 10) * 15000; }
    }

    // DSP variables
    float vF, vD, 
        vZ1, vZ2, vZ3;
    
    public void UpdateBand()
    {
        var f = 2 / 1.85f * Mathf.Sin(Mathf.PI * cutOffFrequency / AudioSettings.outputSampleRate);
        vD = 1 / q;
        vF = (1.85f - 0.75f * vD * f) * f;
    }

    public float[] ApplyFilter(float[] audioData, int channels)
    {
        for (var i = 0; i < audioData.Length; i += channels)
        {
            var si = audioData[i];

            var _vZ1 = 0.5f * si;
            var _vZ3 = vZ2 * vF + vZ3;
            var _vZ2 = (_vZ1 + vZ1 - _vZ3 - vZ2 * vD) * vF + vZ2;

            for (var c = 0; c < channels; c++)
                audioData[i + c] = _vZ2;

            vZ1 = _vZ1;
            vZ2 = _vZ2;
            vZ3 = _vZ3;
        }
        return audioData;
    }
}
