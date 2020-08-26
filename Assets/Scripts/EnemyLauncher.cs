using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLauncher : MonoBehaviour
{
    [SerializeField] GameObject granadePrefab;
    public GameObject playerPrefab;
    [SerializeField] float stillDistanceThreshold = 3f;
    public bool aimAtPlayer = false;
    public bool isBlinking = false;
    public bool isUpShootingLauncher = false;
    [SerializeField] float blinkDistanceThreshold = 0.5f;

    float screenCenter, initializationFactor = 1f, spawnScaleFactor = 0f;
    Granade latestGranadeClass;
    GameObject latestGranade;
    TaskHandler taskHandlerClass;
    Animator anim;
    int counter = 0;

    private void Start()
    {
        var bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        var bottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
        screenCenter = (bottomLeft.x + bottomRight.x)/2f;
        taskHandlerClass = FindObjectOfType<TaskHandler>();
        anim = GetComponent<Animator>();
        if(isBlinking)
            anim.enabled = false;

        if(!isBlinking)
        {
            foreach(Transform child in transform)
            {
                if(child.gameObject.name == "Head")
                {
                    child.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }

    private void Update() {

        if (playerPrefab != null && aimAtPlayer)
        {
            Vector3 target = playerPrefab.transform.position;

            Vector3 objectPos = transform.position;

            if (Vector3.Distance(target, objectPos) <= stillDistanceThreshold && counter % 3 == 0)
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
        else if (playerPrefab != null && isBlinking)
        {
            float targetY = playerPrefab.transform.position.y;

            float objectY = transform.position.y;
            
            if (Mathf.Abs(objectY - targetY) <= blinkDistanceThreshold)
            {
                anim.enabled = true;
            }
        }
    }

    void ShootGranade()
    {
        latestGranadeClass.setGranadeSpeed(initializationFactor);
        latestGranadeClass.MoveGranade();
    }

    void InitiateGranade()
    {
        latestGranade = Instantiate(granadePrefab, transform.position, transform.rotation) as GameObject;
        if (isUpShootingLauncher)
        {
            if(transform.position.x < screenCenter)
                latestGranade.transform.Rotate(0, 0, -90f, Space.World);
            else
                latestGranade.transform.Rotate(0, 0, 90f, Space.World);
                
        }
            
        latestGranadeClass = latestGranade.GetComponent<Granade>();
    }

    public void SetSlowmoForGranadeLauncher(float slowmoFactor)
    {
        initializationFactor = slowmoFactor;
    }

    private void OnDestroy()
    {
        if(latestGranadeClass != null && !latestGranadeClass.isGranadeActive())
            Destroy(latestGranade);
    }

}
