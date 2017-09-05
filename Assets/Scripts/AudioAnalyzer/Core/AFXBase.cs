using System.Collections.Generic;
using UnityEngine;

namespace AudioAnalyzer
{
	abstract public class AFXNormalizedBase : MonoBehaviour
	{
		[SerializeField]	protected BandValueNormalized band;
	}

	abstract public class AFXRangeBase : MonoBehaviour
	{
		[SerializeField] protected BandValueRange band;
	}
}