using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour {

    [SerializeField] float granadeSpeed = 100f;

    void Update() {
        float projectileSpeed = granadeSpeed * Time.deltaTime;
        GetComponent<Rigidbody2D>().velocity = transform.right * projectileSpeed;
    }

    public void setGranadeSpeed(float slowmotionFactor) {
        granadeSpeed *= slowmotionFactor;
    }
}
