using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    [SerializeField] GameObject playerStatistics;
    [SerializeField] float coinValue;
    [SerializeField] string coinType;

    VFXController vfxControllerClass;
    PlayerStatistics playerStatsClass;

    private void Start() {
        vfxControllerClass = FindObjectOfType<VFXController>().GetComponent<VFXController>();
        playerStatsClass = playerStatistics.GetComponent<PlayerStatistics>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Destroy(gameObject);
        vfxControllerClass.InitiateRippleEffect(transform.position);
        playerStatsClass.CoinAcquired(coinValue, coinType);
    }
}
