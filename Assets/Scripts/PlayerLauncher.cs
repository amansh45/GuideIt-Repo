﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLauncher : MonoBehaviour
{

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float startScale = 0f, launcherSize = 0.6f, scalingFactor = 0.05f;
    Granade bulletClass;
    GameObject bulletInstance;
    

    void Start()
    {
        transform.localScale = new Vector3(startScale, startScale, startScale);
        bulletInstance = Instantiate(bulletPrefab, transform.position, transform.rotation) as GameObject;
        bulletClass = bulletInstance.GetComponent<Granade>();
    }

    void Update()
    {
        if (startScale <= launcherSize)
        {
            startScale += scalingFactor;
            transform.localScale = new Vector3(startScale, startScale, startScale);
        }
    }

    public void ShootAndSelfDestruct(Quaternion bulletFiringDirection)
    {
        if (startScale >= launcherSize)
        {
            bulletClass.transform.rotation = transform.rotation;
            bulletClass.setGranadeSpeed(1f);
            bulletClass.MoveGranade();
        }
        else
            Destroy(bulletInstance);

        Destroy(gameObject);
    }
}
