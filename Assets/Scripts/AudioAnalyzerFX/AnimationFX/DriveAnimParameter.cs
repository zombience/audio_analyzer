using System.Collections.Generic;
using UnityEngine;

public class DriveAnimParameter : AudioAnimationBase
{

    [SerializeField]
    protected string targetParam = "animSpeed";
    
        
    protected override void Update()
    {
        anim.SetFloat(targetParam, bandValue);
        
    }
}
