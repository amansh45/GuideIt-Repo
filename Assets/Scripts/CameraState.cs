using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraState : StateMachineBehaviour
{
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CameraTrigger[] arr = System.Enum.GetValues(typeof(CameraTrigger)) as CameraTrigger[];
        
        foreach (CameraTrigger t in arr) {
            animator.ResetTrigger(t.ToString());
        }
    }

}
