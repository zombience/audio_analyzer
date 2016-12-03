using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(AudioSource))]
public class AudioAnalyzer : MonoBehaviour 
{

	public const int BANDS = 4;
	public static float[] output = new float[BANDS]; // averaged amplitude of freqData per band
	public static float[] freqData = new float[sampleCount]; // raw output data of every analyzed sample: values between -1.0 to 1.0
	public static float masterGain = 1f;

#if UNITY_EDITOR
    [SerializeField]
    float[] maxVals = new float[AudioAnalyzer.BANDS];
#endif


    [SerializeField]
	protected int[] crossovers = new int[BANDS]{10, 80, 150, 300}; // split the spectrum into bands at each crossover point 
	[SerializeField]
	protected float[] bandGain = new float[BANDS]{250f, 2500f, 8000f, 15000f}; // multiply the amplitude of each band. higher frequencies require more boosting
	
	[SerializeField]
	protected bool easeAmplitude; // smooth amplitude changes vs. immediate change
	[SerializeField]
	[Range(0.001f, .1f)]
	protected float climbRate;
	[SerializeField]
	[Range(0.001f, .1f)]
	protected float fallRate;
	
	[SerializeField]
	protected bool listen, listenToBuffer, useBakedAudio;

    [SerializeField]
    protected int bufferBand = 0; // which audio band should be sent to preview mixer

	protected bool prevListenState, prevBufferListenState; 

	protected SelectInputGUI micSelector;
	protected AudioSource source;
	[SerializeField]
	protected AudioMixer mixer;

	[SerializeField]
	protected AudioClip clip;
	protected float[] band = new float[BANDS]; // used for accumulating freqData
	protected const int sampleCount = 512; // larger sample sizes will yield more "accurate" analysis at the cost of slower analysis
	protected string selectedDevice;
	protected int minFreq, maxFreq;


    // assign a key here to load the input selection menu on play
    [SerializeField]
    protected KeyCode inputSelectionKey = KeyCode.I;

	//TODO: put this in another thread, see if it improves FPS on mac
	#region Unity Methods
	void Start() 
	{
		
		if(!mixer)
		{
			Debug.LogError("no mixer found on audio analyzer object: mixer must be set in inspector");
			return;
		}

		prevListenState = !listen;
		source = GetComponent<AudioSource>();
		
		mixer.SetFloat("InputVolume", listen ? 0f : -80f);
		source.loop = true; 

		try
		{
			selectedDevice = Microphone.devices[0].ToString();
			Microphone.GetDeviceCaps(selectedDevice, out minFreq, out maxFreq); // get frequency range of device

			if ((minFreq + maxFreq) == 0)
				maxFreq = 48000;

			Debug.Log("selected input device: " + selectedDevice);
		}
		catch 
		{
			Debug.Log("NO AUDIO DEVICE CONNECTED\nattempting to use audio from file");
			useBakedAudio = true;
		}

		if (useBakedAudio && clip != null) {
			source.Stop();
			source.clip = clip;
			source.Play();
		}

		StartCoroutine(ManageBuffer());

#if UNITY_EDITOR

        InitDebugItems();
        //        SceneView.onSceneGUIDelegate += OverlayMeters;
#endif
    }

    //#if UNITY_EDITOR
    //    void OnDestroy()
    //    {
    //        SceneView.onSceneGUIDelegate -= OverlayMeters;
    //    }
    //#endif

    void Update()
	{
        if(!mixer)
		{
			Debug.LogError("no mixer found on audio analyzer object: mixer must be set in inspector");
			return;
		}
		// main audio analysis
		GetMultibandAmplitude();
		
		// select audio input device
		if (Input.GetKeyDown(inputSelectionKey))
		{
			if (micSelector == null)
			{
				micSelector = gameObject.AddComponent<SelectInputGUI>();
				micSelector.SetCallback(SetInputDevice);
			}
			else
			{
				Destroy(micSelector);
				micSelector = null;
			}
		}


		// unity will no longer process audio on a muted AudioSource
		// the mixer must be muted instead
		if(listen == prevListenState)
		{
			prevListenState = !listen;
			mixer.SetFloat("InputVolume", listen ? 0f : -80f);
            if (listen && listenToBuffer)
            {
                listenToBuffer = false;
                prevBufferListenState = true;
                mixer.SetFloat("BufferVolume", -80f);
            }
		}

        if(listenToBuffer == prevBufferListenState)
        {
            prevBufferListenState = !listenToBuffer;
            if(listenToBuffer && listen)
            {
                listen = false;
                prevListenState = true;
                mixer.SetFloat("InputVolume", -80f);
            }

            mixer.SetFloat("BufferVolume", listenToBuffer ? 0f : -80f); 
        }

        


#if UNITY_EDITOR
        WorldMeters();
        for (int i = 0; i < BANDS; i++)
        {
            maxVals[i] = Mathf.Max(maxVals[i], AudioAnalyzer.GetRawOutput(i));
        }

#endif
    }
#endregion

#region API
	public static float GetScaledOutput(int listenBand, float bandMax,float targetMin, float targetMax)
	{
		return output[listenBand].Map(0, bandMax, targetMin, targetMax);
	}
    
    public static float GetRawOutput(int listenBand)
    {
        return output[listenBand];
    }
#endregion


#region audio buffer
	protected void SetInputDevice(string device)
	{
		StopMicrophone();
		selectedDevice = device;
	}

	protected void StopMicrophone () 
	{
		source.Stop();
		Microphone.End(selectedDevice);
	}

	protected IEnumerator ManageBuffer()
	{
		bool usingLiveAudio = false;
		
		while (true)
		{
			if (usingLiveAudio && useBakedAudio)
			{
				if (clip != null)
				{
					source.Stop();
					source.clip = clip;
					source.Play();
					usingLiveAudio = false;
				}
				else
				{
					useBakedAudio = false; // if there's no valid audio file to play, switch back to live
					Debug.Log("no valid audio clip has been assigned");
				}	
			}
			
			while (useBakedAudio)
				yield return null;

			if (!usingLiveAudio)
			{
				source.Stop();
				source.clip = Microphone.Start(selectedDevice, true, 10, maxFreq);
				while (Microphone.GetPosition(selectedDevice) <= 0)
					yield return Microphone.GetPosition(selectedDevice);
				source.Play();
			}

			usingLiveAudio = true;

			SimpleTimer bufferTimer = new SimpleTimer(5f);
			while (!bufferTimer.isFinished && !useBakedAudio)
				yield return bufferTimer;

			// stop playing audio and halt mic recording
			source.Stop(); 
			Microphone.End(selectedDevice);
				
			// set new clip to new recording and wait for recording to begin before playing source
			source.clip = Microphone.Start(selectedDevice, true, 10, maxFreq);
			while (Microphone.GetPosition(selectedDevice) <= 0)
				yield return Microphone.GetPosition(selectedDevice); 

			source.Play();
		}
	}
#endregion


#region audio analysis
	/// <summary>
	/// GetAveragedVolume returns the average volume of the entire signal
	/// </summary>
	protected float GetAverageAmplitude() 
	{
        float[] data = new float[sampleCount];

        float a = 0;
		source.GetSpectrumData(data, 0, FFTWindow.Hamming);

        foreach(float s in data) 
            a += Mathf.Abs(s);
        
        return a/sampleCount;
    }

	protected void GetMultibandAmplitude()
	{
		source.GetSpectrumData(freqData, 0, FFTWindow.Hamming);

		int k = 0;
		int[] bandThresholds = new int[BANDS];
		for(int i = 0; i < BANDS; i++)
		{
			int min = (i > 0 ? crossovers[i-1] : 0); // set the threshold for each band
			bandThresholds[i] = crossovers[i] - min; 
			band[i] = 0f;
#if UNITY_EDITOR
            if(listenToBuffer && i == bufferBand)
            {
                if(bufferSamples.Length != bandThresholds[i])
                {
                    bufferSamples = new float[bandThresholds[i]];
                }
            }

        }

        // IN PROGRESS TODO:
        // trying to pack a buffer with sections of freqData to play back the results of 
        // separating bands, so i can hear what i'm analyzing

        if(listenToBuffer)
        {
            int freqStart = crossovers[bufferBand] - crossovers[Mathf.Max(bufferBand - 1, 0)];
            int freqEnd = bandThresholds[bufferBand];
            Debug.Log("freq start, end, len: " + freqStart + " " + freqEnd + " " + (freqEnd - freqStart));
            for(int i = 0; i < freqEnd - freqStart; i++)
            {
                bufferSamples[i] = freqData[i + freqStart];
            }
            bufferClip = new AudioClip();
            bufferClip.SetData(bufferSamples, 0);
        }
#else 
        }
#endif

        for (int i = 0; i < freqData.Length; i++)
		{
			if (k > BANDS - 1)
				break;

			band[k] += freqData[i]; // sum amplitude of each frequency per band 
            
			if (i > crossovers[k])
			{
				if(easeAmplitude)
				{
					float bandAmp = Mathf.Abs(band[k] / (float)bandThresholds[k]) * bandGain[k]; // divide total amplitude by total number of datapoints = average amplitdue for band
					output[k] = Mathf.Lerp(output[k], bandAmp * masterGain, bandAmp * (bandAmp > output[k] ? climbRate : fallRate)); // if analyzed amplitude is larger than previous amplitude, ease to new amplitude at climbRate, otherwise ease at fallRate
				}
				else
					output[k] = Mathf.Abs(band[k] / (float)bandThresholds[k]) * bandGain[k] * masterGain; 

				k++;
                
                
			}
		}
	}

#if UNITY_EDITOR
    AudioSource bufferSource;
    AudioClip bufferClip = new AudioClip();
    float[] bufferSamples = new float[1];
#endif

#endregion


#if UNITY_EDITOR
    Transform[] meters = new Transform[4];
    Camera c;
    void InitDebugItems()
    {
        
        GameObject parentObj = new GameObject("meters container");
        for(int i = 0; i < meters.Length; i++)
        {
            meters[i] = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            meters[i].position = new Vector3(0, 5000 - i, 0);
            meters[i].localScale = new Vector3(1, 1, 1);
            meters[i].parent = parentObj.transform;
        }
        c = new GameObject("meterCam", typeof(Camera)).GetComponent<Camera>();
        c.transform.position = new Vector3(0, 5020, -40);
        c.transform.parent = parentObj.transform;
        c.clearFlags = CameraClearFlags.Depth;
        c.orthographic = true;
        c.orthographicSize = 24;

        bufferSource = GetComponentInChildren<AudioSource>();
        bufferSource.clip = bufferClip;
    }


    void WorldMeters()
    {
        Vector3 scale = Vector3.one;
        for(int i = 0; i < 4; i++)
        {
            scale.x = AudioAnalyzer.GetRawOutput(i);
            meters[i].localScale = scale;
        }
    }
    //    void OverlayMeters(SceneView view)
    //    {

    //    }
#endif
}



#if UNITY_EDITOR
[CustomEditor(typeof(AudioAnalyzer))]
public class AudioAnalyzerEditor : Editor
{

    AudioAnalyzer obj;
    float[] maxVals = new float[AudioAnalyzer.BANDS];

    void OnEnable()
    {
        obj = target as AudioAnalyzer;
    }
    
    public override void OnInspectorGUI()
    {


        //if(Application.isPlaying)
        //{
        //    for(int i = 0; i < AudioAnalyzer.BANDS; i++)
        //    {
        //        maxVals[i] = Mathf.Max(maxVals[i], AudioAnalyzer.GetRawOutput(i));
                
        //        GUILayout.BeginHorizontal();
        //        GUILayout.Label(string.Format("band {0} max", i));
        //        EditorGUILayout.FloatField(maxVals[i]);
        //        GUILayout.EndHorizontal();
        //    }
        //}
        
        //Rect bands = new Rect(0, 0, 200, 20);
        //bands.width *= .9f;
        //bands.x = 10;
        //bands.y = 10;
        //bands.height = 20;
        //EditorGUI.ProgressBar(bands, AudioAnalyzer.GetScaledOutput(0, 10, 0, 1), "Band 0");
        //bands.y += 20;
        //EditorGUI.ProgressBar(bands, AudioAnalyzer.GetRawOutput(1), "Band 1");
        //bands.y += 20;
        //EditorGUI.ProgressBar(bands, AudioAnalyzer.GetRawOutput(2), "Band 2");
        //bands.y += 20;
        //EditorGUI.ProgressBar(bands, AudioAnalyzer.GetRawOutput(3), "Band 3");

        base.OnInspectorGUI();

    }
}
#endif