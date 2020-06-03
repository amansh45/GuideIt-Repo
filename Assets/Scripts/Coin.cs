using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    VFXController vfxControllerClass;

    private void Start() {
        vfxControllerClass = FindObjectOfType<VFXController>().GetComponent<VFXController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == ObjectsDescription.Player.ToString()) {
            Destroy(gameObject);
            vfxControllerClass.InitiateRippleEffect(transform.position);
        }
    }
}
