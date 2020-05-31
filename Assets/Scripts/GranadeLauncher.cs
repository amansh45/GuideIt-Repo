using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeLauncher : MonoBehaviour {
    
    [SerializeField] GameObject granadePrefab;

    float initializationFactor = 1f;
    Granade granadeClass;

    void ShootGranade() {
        granadeClass.setGranadeSpeed(initializationFactor);
        granadeClass.MoveGranade();
    }

    void InitiateGranade() {
        GameObject granade = Instantiate(granadePrefab, transform.position, transform.rotation) as GameObject;
        granadeClass = granade.GetComponent<Granade>();
    }

    public void SetSlowmoForGranadeLauncher(float slowmoFactor) {
        initializationFactor = slowmoFactor;
    }

}
