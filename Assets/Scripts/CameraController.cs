using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraTrigger
{
    Default,
    Shake,
}

public class CameraController : MonoBehaviour {
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerCamera(CameraTrigger trigger) {
        animator.SetTrigger(trigger.ToString());
    }

}
