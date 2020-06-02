using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLauncher : MonoBehaviour {
    
    [SerializeField] GameObject granadePrefab, playerPrefab;
    
    float initializationFactor = 1f, spawnScaleFactor = 0f;
    Granade granadeClass;
    

    private void Update() {
        Vector3 target = playerPrefab.transform.position;
        target.z = 0f;

        Vector3 objectPos = transform.position;
        target.x = target.x - objectPos.x;
        target.y = target.y - objectPos.y;

        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void ShootGranade() {
        granadeClass.transform.rotation = transform.rotation;
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
