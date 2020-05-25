using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeLauncher : MonoBehaviour {
    
    [SerializeField] GameObject granadePrefab;
    float initializationFactor = 1f;

    void ShootGranade() {
        GameObject granade = Instantiate(granadePrefab, transform.position, transform.rotation) as GameObject;
        granade.GetComponent<Granade>().setGranadeSpeed(initializationFactor);
    }

    public void InitiateGranade(float slowmoFactor) {
        initializationFactor = slowmoFactor;
    }

}
