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
    PlayerStatistics playerStats;
    TaskHandler taskHandler;

    [SerializeField] GameObject coinsAcquired;
    TextMeshProUGUI coinsAcquiredOnScreenText;
    int coinsInScene = 0, currentCoinsAcquired = 0;

    private void Start()
    {
        slowmotionClass = slowmotion.GetComponent<Slowmotion>();
        playerClass = player.GetComponent<Player>();
        playerActionsClass = player.GetComponent<PlayerActions>();
        coinsAcquiredOnScreenText = coinsAcquired.GetComponent<TextMeshProUGUI>();
        coinsInScene = FindObjectsOfType<Coin>().Length;
        coinsAcquiredOnScreenText.text = currentCoinsAcquired.ToString() + " / " + coinsInScene;
        playerStats = FindObjectOfType<PlayerStatistics>();
        taskHandler = FindObjectOfType<TaskHandler>();
    }

    private void Update()
    {
        if (!playerStats.playerStatsLoaded)
            playerStats = FindObjectOfType<PlayerStatistics>();
    }

    public void CoinAcquired(float coinValue, string coinType)
    {
        currentCoinsAcquired += 1;
        coinsAcquiredOnScreenText.text = currentCoinsAcquired.ToString() + " / " + coinsInScene;
        taskHandler.UpdateLevelTaskState(ObjectsDescription.Coin, TaskTypes.Collect, TaskCategory.CountingTask);
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

    private void UpdateAndUnlockNextLevel(int currentChapterIndex, int currentLevelIndex, float timeTaken)
    {
        playerStats.playerCoins += currentCoinsAcquired;
        PlayerStatistics.Level currentLevelObj = playerStats.chaptersList[currentChapterIndex].LevelsInChapter[currentLevelIndex];
        currentLevelObj.IsPlayed = true;
        currentLevelObj.IsPlaying = false;
        currentLevelObj.PersonalBestTime = Mathf.Min(currentLevelObj.PersonalBestTime, timeTaken);
        playerStats.chaptersList[currentChapterIndex].LevelsInChapter[currentLevelIndex] = currentLevelObj;

        if (currentLevelIndex == playerStats.chaptersList[currentChapterIndex].LevelsInChapter.Count - 1)
        {
            if(currentChapterIndex == playerStats.chaptersList.Count - 1)
            {

            } else
            {
                PlayerStatistics.Chapter nextChapterObj = playerStats.chaptersList[currentChapterIndex + 1];
                nextChapterObj.IsLocked = false;
                playerStats.chaptersList[currentChapterIndex + 1] = nextChapterObj;
                PlayerStatistics.Level nextLevelObj = playerStats.chaptersList[currentChapterIndex + 1].LevelsInChapter[0];
                playerStats.highestChapter = PersistentInformation.CurrentChapter = currentChapterIndex + 1;
                if(!nextLevelObj.IsPlayed)
                    nextLevelObj.IsPlaying = true;
                nextLevelObj.IsLocked = false;
                playerStats.chaptersList[currentChapterIndex + 1].LevelsInChapter[0] = nextLevelObj;
            }

        } else
        {
            PlayerStatistics.Level nextLevelObj = playerStats.chaptersList[currentChapterIndex].LevelsInChapter[currentLevelIndex + 1];
            nextLevelObj.IsPlaying = true;
            nextLevelObj.IsLocked = false;
            playerStats.chaptersList[currentChapterIndex].LevelsInChapter[currentLevelIndex + 1] = nextLevelObj;
        }
    }
    

    public void OnLevelFinished(float timeTaken)
    {
        
        string levelName = gameObject.scene.name;
        string[] levelIdentity = levelName.Split('.');
        int currentChapterIndex = int.Parse(levelIdentity[0]);
        int currentLevelIndex = int.Parse(levelIdentity[1]);

        float prevBestTime = playerStats.chaptersList[currentChapterIndex].LevelsInChapter[currentLevelIndex].PersonalBestTime;
        playerStats.levelCompletionData = new PlayerStatistics.LevelCompletionData(currentChapterIndex, currentLevelIndex, coinsInScene, currentCoinsAcquired, timeTaken, prevBestTime);
        UpdateAndUnlockNextLevel(currentChapterIndex, currentLevelIndex, timeTaken);
        
        SceneManager.LoadScene("Level Complete");

    }

    public void LoadNextLevel()
    {
        string levelName = gameObject.scene.name;
        string[] levelIdentity = levelName.Split('.');
        int currentChapterIndex = int.Parse(levelIdentity[0]);
        int currentLevelIndex = int.Parse(levelIdentity[1]);

        if (currentLevelIndex == playerStats.chaptersList[currentChapterIndex].LevelsInChapter.Count - 1)
        {
            if (currentChapterIndex == playerStats.chaptersList.Count - 1)
            {

            }
            else
            {
                SceneManager.LoadScene((currentChapterIndex + 1).ToString() + ".0");
            }

        }
        else
        {
            SceneManager.LoadScene(currentChapterIndex.ToString() + "." + (currentLevelIndex + 1).ToString());
        }

    }

}
