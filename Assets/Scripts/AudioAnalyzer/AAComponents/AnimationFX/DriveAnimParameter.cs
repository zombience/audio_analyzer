using System.Collections.Generic;
using UnityEngine;

public class DriveAnimParameter : AnimationAFXBase
{

    [SerializeField]
    protected string targetParam = "animSpeed";
    
        
    protected override void Update()
    {
        anim.SetFloat(targetParam, bandValue);
        
    }
}
