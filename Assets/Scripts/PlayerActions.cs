using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] float thresholdDistanceBetweenCheckpoints = 0.3f, thresholdBetweenDClickAndPlayer = 0.5f, thresholdDoubleClickTime = 0.3f;
    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject slowmotion;
    [SerializeField] GameObject playerLauncher;
    [SerializeField] Camera mainCamera;
    //ObjectPooler objectPooler;

    float previousClickTime = 0f;

    Player playerClass;
    GameObject currentLine, previousFingerPosition, playerLauncherInstance;
    Vector2 prevPlayerPosition;
    Slowmotion slowmoClass;

    bool isPlayerShooting = false;

    private void Start()
    {
        slowmoClass = slowmotion.GetComponent<Slowmotion>();
        playerClass = GetComponent<Player>();
        //objectPooler = ObjectPooler.Instance;
    }

    private bool IsPlayerTryingToShoot(Vector2 fingerPosition)
    {
        float currentTime = Time.time;
        if (prevPlayerPosition == null)
            return false;
        bool hasClickedNearPlayer = Vector2.Distance(prevPlayerPosition, fingerPosition) <= thresholdBetweenDClickAndPlayer;
        if (currentTime - previousClickTime <= thresholdDoubleClickTime && hasClickedNearPlayer)
        {
            previousClickTime = currentTime;
            return true;
        }
        else
        {
            previousClickTime = currentTime;
            return false;
        }

    }

    float FindAngleBetweenVectors(Vector2 fVec, Vector2 sVec)
    {
        fVec = fVec - sVec;
        float angle = Mathf.Atan2(fVec.y, fVec.x) * Mathf.Rad2Deg;
        return angle;
    }

    private void InitiateShoot()
    {
        playerClass.MovePlayer(PlayerState.Still);
        playerLauncherInstance = Instantiate(playerLauncher, transform.position, transform.rotation);
        playerClass.SetScale(0f);
        isPlayerShooting = true;
    }

    private void ProcessShoot(Vector2 currentFingerPos, Vector2 currentPlayerPosition)
    {
        float launcherAngle = FindAngleBetweenVectors(currentPlayerPosition, currentFingerPos);
        var launcherDirection = Quaternion.Euler(new Vector3(0, 0, launcherAngle));
        playerLauncherInstance.transform.rotation = launcherDirection;
    }

    private void FinalizeShoot(Vector2 currentFingerPos, Vector2 currentPlayerPosition)
    {
        isPlayerShooting = false;
        float bulletFiringAngle = FindAngleBetweenVectors(currentPlayerPosition, currentFingerPos);
        var bulletFiringDirection = Quaternion.Euler(new Vector3(0, 0, bulletFiringAngle));
        playerLauncherInstance.GetComponent<PlayerLauncher>().ShootAndSelfDestruct(bulletFiringDirection);
        playerClass.MovePlayer(PlayerState.Hover);
        playerClass.SetScale(1f);
    }

    private void Update()
    {
        Vector2 tempFingerPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 currentPlayerPos = transform.position;

        if (Input.GetMouseButtonDown(0))
        {
            slowmoClass.updateAnimations(true);
            if (IsPlayerTryingToShoot(tempFingerPos) && playerClass.reachedXPToShoot())
            {
                InitiateShoot();
            }
            prevPlayerPosition = transform.position;
            CreateLine(tempFingerPos);
        }
        else if (Input.GetMouseButton(0))
        {
            if (isPlayerShooting)
            {
                ProcessShoot(tempFingerPos, currentPlayerPos);
            }
            else
            {
                try
                {
                    if (Vector2.Distance(tempFingerPos, previousFingerPosition.transform.position) > thresholdDistanceBetweenCheckpoints)
                        UpdateLine(tempFingerPos);
                }
                catch (System.Exception exception)
                {
                    CreateLine(tempFingerPos);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            slowmoClass.updateAnimations(false);
            if (isPlayerShooting)
            {
                FinalizeShoot(tempFingerPos, currentPlayerPos);
            }
            else
                playerClass.MovePlayer(PlayerState.Run);
        }
    }

    void CreateLine(Vector2 initialFingerPos)
    {
        //currentLine = objectPooler.SpawnFromPool("PathLine", initialFingerPos, Quaternion.identity);
        currentLine = Instantiate(linePrefab, initialFingerPos, Quaternion.identity) as GameObject;
        previousFingerPosition = currentLine;
        playerClass.SetWayPoints(previousFingerPosition, false);
    }


    void UpdateLine(Vector2 newFingerPos)
    {
        float angle = 0;
        /*
        if (numPoints > 1) {
            Vector2 secondVector = newFingerPos - fingerPositions[numPoints - 1];
            Vector2 firstVector = fingerPositions[numPoints - 1] - fingerPositions[numPoints - 2];
            angle = Vector2.Angle(firstVector, secondVector);
        }
        */
        var rotation = Quaternion.Euler(0, 0, angle);
        //GameObject lineInstance = objectPooler.SpawnFromPool("PathLine", newFingerPos, rotation);
        GameObject lineInstance = Instantiate(linePrefab, newFingerPos, rotation) as GameObject;
        previousFingerPosition = lineInstance;
        playerClass.SetWayPoints(previousFingerPosition, true);
    }

}