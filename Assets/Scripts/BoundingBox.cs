using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.name != "Following Missile")
            Destroy(other.gameObject);
    }

}
