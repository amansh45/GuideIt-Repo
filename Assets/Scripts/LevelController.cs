using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [SerializeField] GameObject slowmotion, pauseCanvas, player, finishCanvas, levelCanvas;
    [SerializeField] float onPauseSlowmoFactor = 0.05f;
    Slowmotion slowmotionClass;
    Player playerClass;
    PlayerActions playerActionsClass;

    [SerializeField] GameObject coinsAcquired;
    TextMeshProUGUI coinsAcquiredOnScreenText;
    int coinsInScene = 0, currentCoinsAcquired = 0;

    private void Start()
    {
        slowmotionClass = slowmotion.GetComponent<Slowmotion>();
        playerClass = FindObjectOfType<Player>();
        playerActionsClass = FindObjectOfType<PlayerActions>();
        coinsAcquiredOnScreenText = coinsAcquired.GetComponent<TextMeshProUGUI>();
        coinsInScene = FindObjectsOfType<Coin>().Length;
        coinsAcquiredOnScreenText.text = currentCoinsAcquired.ToString() + " / " + coinsInScene;
    }

    public void CoinAcquired(float coinValue, string coinType)
    {
        currentCoinsAcquired += 1;
        coinsAcquiredOnScreenText.text = currentCoinsAcquired.ToString() + " / " + coinsInScene;
    }

    public void ClickedPauseButton()
    {
        playerClass.MovePlayer(PlayerState.Still);
        slowmotionClass.customSlowmo(true, onPauseSlowmoFactor);
        pauseCanvas.gameObject.SetActive(true);
        playerActionsClass.isGamePaused = true;
    }

    public void ClickedResumeButton()
    {
        playerClass.MovePlayer(PlayerState.Hover);
        slowmotionClass.customSlowmo(false, onPauseSlowmoFactor);
        pauseCanvas.gameObject.SetActive(false);
        playerActionsClass.isGamePaused = false;
    }

    public void ClickedRetryButton()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void OnLevelFinished()
    {
        Destroy(player);
        levelCanvas.gameObject.SetActive(false);
        finishCanvas.gameObject.SetActive(true);
    }

}
