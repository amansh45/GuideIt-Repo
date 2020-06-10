using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [SerializeField] GameObject slowmotion, pauseCanvas, player;
    [SerializeField] float onPauseSlowmoFactor = 0.05f;
    Slowmotion slowmotionClass;
    Player playerClass;
    PlayerActions playerActionsClass;

    private void Start()
    {
        slowmotionClass = slowmotion.GetComponent<Slowmotion>();
        playerClass = FindObjectOfType<Player>();
        playerActionsClass = FindObjectOfType<PlayerActions>();
        Debug.Log("Started from level Controller...");
    }

    public void clickedPauseButton()
    {
        playerClass.MovePlayer(PlayerState.Still);
        slowmotionClass.customSlowmo(true, onPauseSlowmoFactor);
        pauseCanvas.gameObject.SetActive(true);
        playerActionsClass.isGamePaused = true;
    }

    public void clickedResumeButton()
    {
        playerClass.MovePlayer(PlayerState.Hover);
        slowmotionClass.customSlowmo(false, onPauseSlowmoFactor);
        pauseCanvas.gameObject.SetActive(false);
        playerActionsClass.isGamePaused = false;
    }

    public void clickedRetryButton()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    
}
