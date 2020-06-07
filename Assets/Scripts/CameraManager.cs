using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectsDescription
{
    Player,
    EnemyObject,
    EnemyProjectile,
    PlayerProjectile,
    EnemyLauncher,
    FinishLine,
}


public class CameraManager : MonoBehaviour
{
    [SerializeField] CameraController camController;
    private Coroutine routine;

    private void Awake()
    {
        int cameraManagerCount = FindObjectsOfType<CameraManager>().Length;
        if (cameraManagerCount > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    IEnumerator CamShake(float sec)
    {
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
