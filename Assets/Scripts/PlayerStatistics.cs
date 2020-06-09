using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStatistics : MonoBehaviour
{
    [SerializeField] GameObject coinsAcquired;
    TextMeshProUGUI coinsAcquiredOnScreenText;
    int coinsInScene = 0, currentCoinsAcquired = 0;

    private void Start()
    {
        coinsAcquiredOnScreenText = coinsAcquired.GetComponent<TextMeshProUGUI>();
        coinsInScene = FindObjectsOfType<Coin>().Length;
        coinsAcquiredOnScreenText.text = currentCoinsAcquired.ToString() + " / " + coinsInScene;
    }

    public void CoinAcquired(float coinValue, string coinType)
    {
        currentCoinsAcquired += 1;
        coinsAcquiredOnScreenText.text = currentCoinsAcquired.ToString() + " / " + coinsInScene;
    }
}
