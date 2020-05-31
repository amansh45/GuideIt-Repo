using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour {

    [SerializeField] float granadeSpeed = 100f, shrinkFactorOnLaunch = 0.6f, explosionTime = 1.5f;
    VFXController vfxControllerClass;

    bool isGranadeMoving = false;
    float startScale = 0, scaleFactor = 0.01f;

    private void Start() {
        transform.localScale = new Vector3(startScale, startScale, startScale);
        vfxControllerClass = FindObjectOfType<VFXController>().GetComponent<VFXController>();
    }

    void Update() {
        if(isGranadeMoving) {
            float projectileSpeed = granadeSpeed * Time.deltaTime;
            GetComponent<Rigidbody2D>().velocity = transform.right * projectileSpeed;
        } else if(startScale <= 1f) {
            startScale += scaleFactor;
            transform.localScale = new Vector3(startScale, startScale, startScale);
        }
    }

    
    private void OnTriggerEnter2D(Collider2D other) {
        DeleteThis delClass = other.gameObject.GetComponent<DeleteThis>();
        if(delClass != null) {
            Destroy(gameObject);
            vfxControllerClass.InitiateExplodeEffect(transform.position, explosionTime);
        }
    }

    public void SetScaleFactor(float slomotionScaleFactor) {
        scaleFactor *= slomotionScaleFactor;
    }

    public void MoveGranade() {
        isGranadeMoving = true;
        transform.localScale -= new Vector3(0, shrinkFactorOnLaunch, 0);
    }

    public void setGranadeSpeed(float slowmotionFactor) {
        granadeSpeed *= slowmotionFactor;
    }

    public bool isGranadeActive() {
        return isGranadeMoving;
    }

}
