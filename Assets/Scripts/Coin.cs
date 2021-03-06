﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    [SerializeField] GameObject levelController;
    [SerializeField] float coinValue;
    [SerializeField] string coinType;

    VFXController vfxControllerClass;
    LevelController levelControllerClass;

    private void Start() {
        vfxControllerClass = FindObjectOfType<VFXController>().GetComponent<VFXController>();
        levelControllerClass = levelController.GetComponent<LevelController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.name == ObjectsDescription.Player.ToString())
        {
            Destroy(gameObject);
            levelControllerClass.CoinAcquired(coinValue, coinType);
            vfxControllerClass.InitiateRippleEffect(transform.position);
        }
    }
}
