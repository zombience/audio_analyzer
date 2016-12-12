using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationAFXBase : AFXBase
{

    protected Animator anim;
    protected AnimatorStateInfo currentState, previousState;
    
	protected virtual void Start ()
    {
        anim = GetComponent<Animator>();
        previousState = currentState = anim.GetCurrentAnimatorStateInfo(0);
	}

    protected virtual void Update()
    {

    }
}
