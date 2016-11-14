using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
 
[RequireComponent(typeof(AudioSource))]
public class AudioAnalyzer : MonoBehaviour 
{

	public const int BANDS = 4;
	public static float[] output = new float[BANDS]; // averaged amplitude of freqData per band
	public static float[] freqData = new float[sampleCount]; // raw output data of every analyzed sample: values between -1.0 to 1.0
	public static float masterGain = 1f;


	protected float mGain { get { return masterGain; } }
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
	protected bool listen, useBakedAudio;

	protected bool isListening; 

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

		isListening = !listen;
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
    }

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
		if(listen == isListening)
		{
			isListening = !listen;
			mixer.SetFloat("InputVolume", listen ? 0f : -80f);
		}
	}
	#endregion

	#region API
	public static float GetScaledOutput(int listenBand, float bandMax,float targetMin, float targetMax)
	{
		return output[listenBand].Map(0, bandMax, targetMin, targetMax);
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
		float[] bandThresholds = new float[BANDS];
		for(int i = 0; i < BANDS; i++)
		{
			float min = (i > 0 ? crossovers[i-1] : 0); // set the threshold for each band
			bandThresholds[i] = crossovers[i] - min; 
			band[i] = 0f;
		}

		for (int i = 0; i < freqData.Length; i++)
		{
			if (k > BANDS - 1)
				break;

			band[k] += freqData[i]; // sum amplitude of each frequency per band 

			if (i > crossovers[k])
			{
				if(easeAmplitude)
				{
					float bandAmp = Mathf.Abs(band[k] / bandThresholds[k]) * bandGain[k]; // divide total amplitude by total number of datapoints = average amplitdue for band
					output[k] = Mathf.Lerp(output[k], bandAmp * masterGain, bandAmp * (bandAmp > output[k] ? climbRate : fallRate)); // if analyzed amplitude is larger than previous amplitude, ease to new amplitude at climbRate, otherwise ease at fallRate
				}
				else
					output[k] = Mathf.Abs(band[k] / bandThresholds[k]) * bandGain[k] * masterGain; 

				k++;
			}
		}
	}
	#endregion
}
