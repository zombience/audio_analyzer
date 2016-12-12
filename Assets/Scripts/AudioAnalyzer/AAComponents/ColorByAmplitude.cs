using UnityEngine;
using System.Collections;

public class ColorByAmplitude : AFXBase
{
	#region vars
	[SerializeField]
	protected Color high;
	[SerializeField]
	protected Color low;

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
			mats[i].color = Color.Lerp(low, high, bandValue);
		}   
		rend.materials = mats;
	}
	#endregion
}
