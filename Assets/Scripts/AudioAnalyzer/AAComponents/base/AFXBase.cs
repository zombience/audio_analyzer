using UnityEngine;


public class AFXBase : MonoBehaviour
{
    // choose which audio band to drive interactions
    [SerializeField]
    protected int band;

    [SerializeField]
    protected bool easeRise, easeFall;

    // set the range of min/max to achieve desired range of effect
    // rise / fallrate 
    [SerializeField]
    protected float minOutput = 0f, maxOutput = 1f,
        fallRate = 1f, riseRate = 1f;

    public float bandValue
    {
        get
        {
            float newVal = AudioAnalyzer.GetScaledOutput(band, minOutput, maxOutput);
            if(easeRise && newVal > _bandValue)
            {
                _bandValue = Mathf.Lerp(_bandValue, newVal, Time.deltaTime * riseRate);
            }
            else if(easeFall && newVal < _bandValue)
            {
                _bandValue = Mathf.Lerp(_bandValue, newVal, Time.deltaTime * fallRate);
            }
            else
            {
                _bandValue = newVal;
            }
            return _bandValue;
        }
    }
        
    protected float _bandValue;
}

    

