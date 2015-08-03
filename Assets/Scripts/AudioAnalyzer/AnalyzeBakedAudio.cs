using UnityEngine;
using System.Collections;

public class AnalyzeBakedAudio : MonoBehaviour 
{
	#region vars

	public AudioSource source;
	public float[] output = new float[BANDS];
	public float [] curve = new float[BANDS];
	public int[] crossovers = new int[BANDS];


	private const int BANDS = 4;

	private float[] freqData = new float[8192];

	private float[] band;
	private int index = 0;


	#endregion
	
	#region Unity methods
	void Start () 
	{	
		band = new float[BANDS];
		output = new float[BANDS];
		crossovers[3] = freqData.Length - 1000;
		
	}
	
	void Update () 
	{
		Analyze();
		for(int i = 0; i < BANDS; i++)
			output[i] *= curve[i];
		
	}
	#endregion

	#region Actions

	protected void StartAudio()
	{
		source.Play();
	}

	protected void Analyze()
	{
		source.GetSpectrumData(freqData, 0, FFTWindow.Hamming);

		//bool cutoff = false;
		int k = 0;
		float[] lengths = new float[BANDS];
		for(int i = 0; i < BANDS; i++)
		{
			float min = (i > 0 ? crossovers[i-1] : 0);
			lengths[i] = crossovers[i] - min; 
			band[i] = 0f;
		}
		
		for (int i = 0; i < freqData.Length; i++)
		{
			if (k > BANDS - 1)
				break;

			band[k] += freqData[i];
			if(i > crossovers[k])
			{
				output[k] = Mathf.Abs(band[k] / lengths[k]);
				k++;
			}
			/*
			float d = freqData[i];
			float b = band[k];
			band[k] = (d > b) ? d : b;
			if (i > crossovers[k] - 10)
			{
				if (cutoff)
					break;
				
				output[k] = band[k];
				band[k] = 0;
				
				k++;
				if (i > crossovers[BANDS - 1] - 10)
					cutoff = true;
			}*/
		}
	}
	#endregion
}
