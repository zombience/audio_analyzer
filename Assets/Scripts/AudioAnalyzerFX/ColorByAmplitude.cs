using UnityEngine;
using System.Collections;

public class ColorByAmplitude : MonoBehaviour 
{
	#region vars
	[SerializeField]
	protected Color high;
	[SerializeField]
	protected Color low;
	[SerializeField]
	[Range(0,3)]
	protected int band;
	[SerializeField]
	protected float maxInput = 33f;

	protected Renderer rend;
	protected Material[] mats;
	#endregion
	
	#region Unity methods
	void Start () 
	{	
		rend = GetComponent<Renderer>();
		mats = rend.materials;
	}
	
	void Update () 
	{
		for(int i = 0; i < mats.Length; i++)
		{
			mats[i].color = Color.Lerp(low, high, AudioAnalyzer.GetScaledOutput(band, maxInput, .1f, 1f));
		}
		rend.materials = mats;
	}
	#endregion
}
