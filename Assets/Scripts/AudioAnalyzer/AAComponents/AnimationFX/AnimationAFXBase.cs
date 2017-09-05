using UnityEngine;
namespace AudioAnalyzer
{
	[RequireComponent(typeof(Animator))]
	public class AnimationAFXBase : AFXRangeBase
	{

		protected Animator anim;
		protected AnimatorStateInfo currentState, previousState;

		protected void Start()
		{
			anim = GetComponent<Animator>();
			previousState = currentState = anim.GetCurrentAnimatorStateInfo(0);
		}
	}
}