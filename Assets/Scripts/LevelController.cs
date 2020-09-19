using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField] GameObject slowmotion, pauseCanvas, retryCanvas, levelCompleteSlider, levelCompletedTextGO, player, playSpace, gameLines, firstTaskGO, secondTaskGO;
    [SerializeField] float onPauseSlowmoFactor = 0.05f, movementOffset = 2f, timeForAdButtonAnimation = 7f;
    [SerializeField] AudioClip playerDeathSFX;
    [SerializeField] AudioClip coinsAcquiredSFX;
    [SerializeField] GameObject coinsAcquired;
    [SerializeField] Image retryCanvasAdButton, retryCanvasNextToAdButton;

    Slowmotion slowmotionClass;
    Player playerClass;
    PlayerActions playerActionsClass;
    PlayerStatistics playerStats;
    TaskHandler taskHandler;
    PolygonCollider2D playSpaceCollider;
    TextMeshProUGUI levelCompletedText;
    GameLines gameLinesClass;
    float initialTimeForAdButtonAnimation;
    float lowerBound, prevLowerBound;

    GameObject testObj = null;
    bool isPlayerStillBeforePause = false;

    TextMeshProUGUI coinsAcquiredOnScreenText;
    public int coinsInScene = 0;
    public int currentCoinsAcquired = 0;
    ProceduralGeneration pg;
    bool retryCanvasVisible = false, retryCanvasAnimationDisabled = false;

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
        prevLowerBound = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        playSpaceCollider = playSpace.GetComponent<PolygonCollider2D>();
        gameLinesClass = gameLines.GetComponent<GameLines>();
        levelCompletedText = levelCompletedTextGO.GetComponent<TextMeshProUGUI>();
        PersistentInformation.LevelIdentifier = gameObject.scene.name;
        pg = FindObjectOfType<ProceduralGeneration>();
        initialTimeForAdButtonAnimation = timeForAdButtonAnimation;
    }

    private void UpdateFirstTaskOnScreen(bool isTaskCompleted)
    {
        int findex = playerStats.firstActiveTaskIndex;
        PlayerStatistics.Task firstTask;
        if (findex != -1)
        {
            firstTask = taskHandler.firstTask;

            if (isTaskCompleted)
                playerStats.playerCoins += firstTask.TaskCompletionAward;

            foreach (Transform child in firstTaskGO.gameObject.transform)
            {
                if (child.gameObject.name == "Text")
                {
                    var tmpTask = child.gameObject.GetComponent<TextMeshProUGUI>();
                    if (firstTask.CurrTaskCategory == TaskCategory.CountingTask)
                        tmpTask.text = firstTask.TaskDescription + "\n" + firstTask.CurrentCount + " / " + firstTask.CountLimit;
                    else
                        tmpTask.text = firstTask.TaskDescription;
                }

            }
        }
    }

    private void UpdateSecondTaskOnScreen(bool isTaskCompleted)
    {
        PlayerStatistics.Task secondTask;
        int sindex = playerStats.secondActiveTaskIndex;
        if (sindex != -1)
        {
            secondTask = taskHandler.secondTask;

            if (isTaskCompleted)
                playerStats.playerCoins += secondTask.TaskCompletionAward;

            foreach (Transform child in secondTaskGO.transform)
            {
                if (child.gameObject.name == "Text")
                {
                    var tmpTask = child.gameObject.GetComponent<TextMeshProUGUI>();
                    if (secondTask.CurrTaskCategory == TaskCategory.CountingTask)
                        tmpTask.text = secondTask.TaskDescription + "\n" + secondTask.CurrentCount + " / " + secondTask.CountLimit;
                    else
                        tmpTask.text = secondTask.TaskDescription;
                }
                
            }
        }
    }

    private void Update()
    {
        if(retryCanvasVisible)
        {
            timeForAdButtonAnimation -= Time.deltaTime;
            if (timeForAdButtonAnimation > 0)
                retryCanvasAdButton.fillAmount = (timeForAdButtonAnimation / initialTimeForAdButtonAnimation);
            else if(!retryCanvasAnimationDisabled)
            {
                retryCanvasAdButton.gameObject.GetComponent<Animator>().enabled = false;
                retryCanvasAnimationDisabled = true;
                foreach(Transform child in retryCanvasAdButton.gameObject.transform)
                {
                    if (child.gameObject.name == "Retry Icon")
                        child.gameObject.SetActive(true);
                    else if (child.gameObject.name == "WatchAd Icon")
                        child.gameObject.SetActive(false);
                }

                foreach(Transform child in retryCanvasNextToAdButton.gameObject.transform)
                {
                    if (child.gameObject.name == "Retry Icon")
                        child.gameObject.SetActive(false);
                    else if (child.gameObject.name == "Upgrades")
                        child.gameObject.SetActive(true);
                }
            }
        }
    }

    IEnumerator PlayerDeathActions(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        
        if (pg != null)
            ClickedRetryButton(true);
        else
        {
        
            slowmotionClass.customSlowmo(true, onPauseSlowmoFactor);
            retryCanvas.gameObject.SetActive(true);
            retryCanvasVisible = true;
            levelCompleteSlider.GetComponent<Slider>().value = gameLinesClass.levelCompleted;
            int completedPercentage = (int)(gameLinesClass.levelCompleted * 100f);
            levelCompletedText.text = "Level Completed: " + completedPercentage + "%";
            playerActionsClass.isGamePaused = true;
        }
    }

    public void ShowRetryCanvas(float waitTime)
    {
        if (pg != null)
            playerStats.prevProceduralLevelFailed = true;

        string levelName = gameObject.scene.name;
        string[] levelIdentity = levelName.Split('.');
        int currentChapterIndex = int.Parse(levelIdentity[0]);
        int currentLevelIndex = int.Parse(levelIdentity[1]);

        PlayerStatistics.Level currentLevel = playerStats.chaptersList[currentChapterIndex].LevelsInChapter[currentLevelIndex];
        if(!currentLevel.IsPlayed)
        {
            currentLevel.numTimesLevelFailed += 1;
            currentLevel.numTimesNearMiss = 0;
            playerStats.chaptersList[currentChapterIndex].LevelsInChapter[currentLevelIndex] = currentLevel;
        }

        AudioSource.PlayClipAtPoint(playerDeathSFX, Camera.main.transform.position, playerStats.sfxVolume);

        StartCoroutine(PlayerDeathActions(waitTime));
    }

    public void CoinAcquired(float coinValue, string coinType)
    {
        currentCoinsAcquired += 1;
        coinsAcquiredOnScreenText.text = currentCoinsAcquired.ToString() + " / " + coinsInScene;
        AudioSource.PlayClipAtPoint(coinsAcquiredSFX, Camera.main.transform.position, playerStats.sfxVolume);
        taskHandler.UpdateLevelTaskState(ObjectsDescription.Coin, TaskTypes.Collect, TaskCategory.CountingTask, new List<string>() { });
    }

    public void ClickedPauseButton()
    {
        if (playerClass.playerState == PlayerState.Still)
        {
            isPlayerStillBeforePause = true;
        }
        else
        {
            playerClass.playerState = PlayerState.Still;
            isPlayerStillBeforePause = false;
        }

        slowmotionClass.customSlowmo(true, onPauseSlowmoFactor);
        pauseCanvas.gameObject.SetActive(true);
        UpdateFirstTaskOnScreen(false);
        UpdateSecondTaskOnScreen(false);
        playerActionsClass.isGamePaused = true;
    }

    public void ClickedResumeButton()
    {
        slowmotionClass.customSlowmo(false, onPauseSlowmoFactor);
        pauseCanvas.gameObject.SetActive(false);
        playerActionsClass.isGamePaused = false;
        if (!isPlayerStillBeforePause)
        {
            playerClass.playerState = PlayerState.Hover;
            Debug.Log("Player was not still before pause..");
        }
        else
            playerClass.playerState = PlayerState.Still;
    }

    public void ExecuteResetTaskRequest()
    {
        taskHandler.ResetTasks();
    }

    public void ClickedRetryButton(bool taskResetRequired)
    {
        if(taskResetRequired)
            taskHandler.ResetTasks();
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
        currentLevelObj.CoinsInLevel = coinsInScene;
        currentLevelObj.CoinsAcquiredInLevel = currentCoinsAcquired;
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
        
        if(pg != null)
        {
            ClickedRetryButton(false);
            playerStats.listOfObjects.Clear();
        } else
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
        
        LoadSaveStats.SavePlayerData(playerStats.tasksList, playerStats.chaptersList, playerStats.upgradesList, playerStats.firstActiveTaskIndex, playerStats.secondActiveTaskIndex, playerStats.tasksCompleted,
            playerStats.highestChapter, playerStats.highestLevel, playerStats.playerCoins, playerStats.musicVolume, playerStats.sfxVolume);
    }

}
