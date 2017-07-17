using UnityEngine;
namespace AudioAnalyzer
{
	[RequireComponent(typeof(Animator))]
	public class AnimationAFXBase : AFXBase
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