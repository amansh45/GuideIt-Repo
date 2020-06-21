using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectsDescription
{
    Player,
    EnemyObject,
    Box,
    Blade,
    Square,
    EnemyProjectile,
    NearMissBoundary,
    PlayerProjectile,
    EnemyLauncher,
    PlayerLauncher,
    FinishLine,
    Coin,
}


public class CameraManager : MonoBehaviour
{
    public CameraController camController;
    private Coroutine routine;

    private void Awake()
    {
        camController = FindObjectOfType<CameraController>();
        int cameraManagerCount = FindObjectsOfType<CameraManager>().Length;
        if (cameraManagerCount > 1)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    IEnumerator CamShake(float sec)
    {
        if (camController == null)
            camController = FindObjectOfType<CameraController>();

        camController.TriggerCamera(CameraTrigger.Shake);
        yield return new WaitForSeconds(sec);
        camController.TriggerCamera(CameraTrigger.Default);
    }

    public void ShakeCamera(float sec)
    {
        if (routine != null)
            StopCoroutine(routine);
        routine = StartCoroutine(CamShake(sec));
    }
}
