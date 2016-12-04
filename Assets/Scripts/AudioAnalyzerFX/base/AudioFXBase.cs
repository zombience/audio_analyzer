using UnityEngine;


public class AudioFXBase : MonoBehaviour
{
    
    // choose which audio band to drive interactions
    [SerializeField]
    protected int band;

    // min/max input sets the scale that this script expects to see 
    // from AudioAnalyzer. min/max in can be tweaked alongside min/max out
    // in order to achieve best output range, depending on audio signal
    [SerializeField]
    protected float minOutput = 0f, maxOutput = 1f;

    public float bandValue { get { return AudioAnalyzer.GetScaledOutput(band, minOutput, maxOutput); } }
}

    

