using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLauncher : MonoBehaviour
{

    [SerializeField] GameObject granadePrefab, playerPrefab;
    [SerializeField] float distanceThreshold = 3f;

    float initializationFactor = 1f, spawnScaleFactor = 0f;
    Granade latestGranadeClass;
    GameObject latestGranade;
    TaskHandler taskHandlerClass;
    int counter = 0;

    private void Start()
    {
        taskHandlerClass = FindObjectOfType<TaskHandler>();
    }

    private void Update() {

        if(playerPrefab != null)
        {
            Vector3 target = playerPrefab.transform.position;

            Vector3 objectPos = transform.position;

            if (Vector3.Distance(target, objectPos) <= distanceThreshold && counter % 3 == 0)
            {
                target.x = target.x - objectPos.x;
                target.y = target.y - objectPos.y;

                float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                if (counter == 10)
                    counter = 1;
            }
            counter += 1;
        }
    }

    void ShootGranade()
    {
        latestGranadeClass.transform.rotation = transform.rotation;
        latestGranadeClass.setGranadeSpeed(initializationFactor);
        latestGranadeClass.MoveGranade();
    }

    void InitiateGranade()
    {
        latestGranade = Instantiate(granadePrefab, transform.position, transform.rotation) as GameObject;
        latestGranadeClass = latestGranade.GetComponent<Granade>();
    }

    public void SetSlowmoForGranadeLauncher(float slowmoFactor)
    {
        initializationFactor = slowmoFactor;
    }

    private void OnDestroy()
    {
        taskHandlerClass.UpdateLevelTaskState(ObjectsDescription.EnemyLauncher, TaskTypes.Destroy, TaskCategory.CountingTask);
        if(!latestGranadeClass.isGranadeActive())
            Destroy(latestGranade);
    }

}
