using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {
    [SerializeField] float rippleEffectDuration = 1f;

    float durationOfCoinRippleEffect = 2f;
    VFXController vfxControllerClass;
    private void Start() {
        vfxControllerClass = FindObjectOfType<VFXController>().GetComponent<VFXController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.GetComponent<Player>() != null) {
            Destroy(gameObject);
            vfxControllerClass.InitiateRippleEffect(transform.position, rippleEffectDuration);
        }
    }
}
