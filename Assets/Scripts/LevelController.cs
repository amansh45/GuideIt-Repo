using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField] GameObject slowmotion, pauseCanvas, retryCanvas, levelCompleteSlider, levelCompletedTextGO, player, playSpace, gameLines, firstTaskGO, secondTaskGO;
    [SerializeField] float onPauseSlowmoFactor = 0.05f, movementOffset = 2f;
    Slowmotion slowmotionClass;
    Player playerClass;
    PlayerActions playerActionsClass;
    PlayerStatistics playerStats;
    TaskHandler taskHandler;
    PolygonCollider2D playSpaceCollider;
    TextMeshProUGUI levelCompletedText;
    GameLines gameLinesClass;
    float lowerBound, prevLowerBound;

    GameObject testObj = null;
    bool isPlayerStillBeforePause = false;

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
        prevLowerBound = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        playSpaceCollider = playSpace.GetComponent<PolygonCollider2D>();
        gameLinesClass = gameLines.GetComponent<GameLines>();
        levelCompletedText = levelCompletedTextGO.GetComponent<TextMeshProUGUI>();
    }

    /*
    private void SetBoundingBox()
    {
        // lower bound position of camera.
        lowerBound = Mathf.Min(lowerBound, player.transform.position.y - movementOffset);

        if(testObj == null)
        {
            testObj = Instantiate(testPrefab, playSpaceCollider.bounds.extents, transform.rotation);
            testObj.name = "extents_test";
            testObj = Instantiate(testPrefab, playSpaceCollider.bounds.center, transform.rotation);
            testObj.name = "centers_test";
            testObj = Instantiate(testPrefab, playSpaceCollider.bounds.min, transform.rotation);
            testObj.name = "min_test";
            testObj = Instantiate(testPrefab, playSpaceCollider.bounds.max, transform.rotation);
            testObj.name = "max_test";
        }

        Debug.Log(playSpaceCollider.bounds.extents);

        //playSpaceCollider.bounds.extents = new Vector3(lowerBound, playSpace.transform.position.x)
        //Debug.Log("extents : " + playSpaceCollider.bounds.extents);
    }
    */

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
        //SetBoundingBox();
    }

    IEnumerator PlayerDeathActions(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        slowmotionClass.customSlowmo(true, onPauseSlowmoFactor);
        retryCanvas.gameObject.SetActive(true);
        levelCompleteSlider.GetComponent<Slider>().value = gameLinesClass.levelCompleted;
        int completedPercentage = (int) (gameLinesClass.levelCompleted * 100f);
        levelCompletedText.text = "Level Completed: " + completedPercentage + "%";
        playerActionsClass.isGamePaused = true;
    }

    public void ShowRetryCanvas(float waitTime)
    {
        StartCoroutine(PlayerDeathActions(waitTime));
    }

    public void CoinAcquired(float coinValue, string coinType)
    {
        currentCoinsAcquired += 1;
        coinsAcquiredOnScreenText.text = currentCoinsAcquired.ToString() + " / " + coinsInScene;
        taskHandler.UpdateLevelTaskState(ObjectsDescription.Coin, TaskTypes.Collect, TaskCategory.CountingTask);
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

    public void ClickedRetryButton()
    {
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
