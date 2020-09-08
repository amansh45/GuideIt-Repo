using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

// types of tasks.
public enum TaskTypes
{
    NearMiss,
    Collect,
    Destroy,
    UpdateSkin,
    Follow,
    Hover,
    ScreenClick,
    NoHit,
    CollectAllCoinsInLevel,
    NoNearMiss,
    Collide
}

public enum TaskCategory
{
    CountingTask,
    ImmediateActionTask,
    TimerTask,
}

public class TaskHandler : MonoBehaviour
{
    /*
    launcher -> {
                    nearMiss ->  CountingTask,
                    destroy  ->  CountingTask,
                }
    */

    PlayerStatistics playerStats;
    public PlayerStatistics.Task firstTask, secondTask;
    bool firstTimeLoad = true;
    PlayerStatistics.Task firstTaskBackup, secondTaskBackup;
    int currentChapterIndex, currentLevelIndex;
    string sceneName;

    private void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name;
        if(sceneName.Contains("."))
        {
            string[] levelChapterIndex = sceneName.Split('.');
            currentChapterIndex = int.Parse(levelChapterIndex[0]);
            currentLevelIndex = int.Parse(levelChapterIndex[1]);
        }

        int taskHandlerCount = FindObjectsOfType<TaskHandler>().Length;
        if (taskHandlerCount > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStatistics>();
    }

    public void UpdateTaskPointers()
    {
        int findex = playerStats.firstActiveTaskIndex, sindex = playerStats.secondActiveTaskIndex;

        if (findex != -1)
        {
            firstTask = playerStats.tasksList[findex];
            firstTaskBackup = firstTask;
        }

        if (sindex != -1)
        {
            secondTask = playerStats.tasksList[sindex];
            secondTaskBackup = secondTask;
        }
    }


    private void Update()
    {
        if(firstTimeLoad && playerStats.isActiveAndEnabled)
        {
            UpdateTaskPointers();
            firstTimeLoad = false;
        }
    }

    private bool IsLevelEligibleToExecuteTask()
    {
        PlayerStatistics.Level levelData = playerStats.chaptersList[currentChapterIndex].LevelsInChapter[currentLevelIndex];
        return !levelData.IsPlayed;
    }

    private bool UpdateCameFromLevel()
    {
        return sceneName.Contains(".");
    }




    // real time updation will be done by mainmenuhandler or levelcontroller
    // level needs to be completed for updating the task
    // after level complete levelTask will be completed fully or not at all
    // after level complete gameTask can be completed partially.
    public void UpdateLevelTaskState(ObjectsDescription objectType, TaskTypes taskType, TaskCategory taskCategory, List<string> metaData)
    {
        if (UpdateCameFromLevel() && !IsLevelEligibleToExecuteTask())
            return;

        bool objTypeMatchesFirstTask = Array.Exists(firstTask.AssociatedWith, element => element == objectType);
        bool objTypeMatchesSecondTask = Array.Exists(secondTask.AssociatedWith, element => element == objectType);

        if (taskCategory == TaskCategory.CountingTask)
        {
            
            if(objTypeMatchesFirstTask && firstTask.CurrTaskType == taskType && firstTask.CurrTaskCategory == taskCategory)
            {
                if (firstTask.CurrentCount < firstTask.CountLimit)
                    firstTask.CurrentCount += 1;
                
                if(firstTask.CurrentCount == firstTask.CountLimit)
                    firstTask.IsCompleted = true;
            }

            if (objTypeMatchesSecondTask && secondTask.CurrTaskType == taskType && secondTask.CurrTaskCategory == taskCategory)
            {
                if (secondTask.CurrentCount < secondTask.CountLimit)
                    secondTask.CurrentCount += 1;
                
                if(secondTask.CurrentCount == secondTask.CountLimit)
                    secondTask.IsCompleted = true;
            }

            if(taskType == TaskTypes.NearMiss)
            {
                Debug.Log("Near miss registered...");
                PlayerStatistics.Level levelData = playerStats.chaptersList[currentChapterIndex].LevelsInChapter[currentLevelIndex];
                levelData.numTimesNearMiss += 1;
                playerStats.chaptersList[currentChapterIndex].LevelsInChapter[currentLevelIndex] = levelData;
            }

        } else if(taskCategory == TaskCategory.ImmediateActionTask)
        {
            if(taskType == TaskTypes.CollectAllCoinsInLevel)
            {
                if (objTypeMatchesFirstTask && firstTask.CurrTaskType == taskType && firstTask.CurrTaskCategory == taskCategory)
                {
                    if (int.Parse(metaData[0]) == int.Parse(metaData[1]))
                        firstTask.IsCompleted = true;
                }

                if (objTypeMatchesSecondTask && secondTask.CurrTaskType == taskType && secondTask.CurrTaskCategory == taskCategory)
                {
                    if (int.Parse(metaData[0]) == int.Parse(metaData[1]))
                        secondTask.IsCompleted = true;
                }
            } else if(taskType == TaskTypes.Hover || taskType == TaskTypes.NoHit)
            {
                if (objTypeMatchesFirstTask && firstTask.CurrTaskType == taskType && firstTask.CurrTaskCategory == taskCategory)
                {
                    firstTask.IsCompleted = bool.Parse(metaData[0]);
                }

                if (objTypeMatchesSecondTask && secondTask.CurrTaskType == taskType && secondTask.CurrTaskCategory == taskCategory)
                {
                    secondTask.IsCompleted = bool.Parse(metaData[0]);
                }
            } else if(taskType == TaskTypes.NoNearMiss)
            {
                if (objTypeMatchesFirstTask && firstTask.CurrTaskType == taskType && firstTask.CurrTaskCategory == taskCategory)
                {
                    firstTask.IsCompleted = (playerStats.chaptersList[currentChapterIndex].LevelsInChapter[currentLevelIndex].numTimesNearMiss == 0);
                }

                if (objTypeMatchesSecondTask && secondTask.CurrTaskType == taskType && secondTask.CurrTaskCategory == taskCategory)
                {
                    secondTask.IsCompleted = (playerStats.chaptersList[currentChapterIndex].LevelsInChapter[currentLevelIndex].numTimesNearMiss == 0);
                }
            } else if(taskType == TaskTypes.UpdateSkin)
            {
                if (objTypeMatchesFirstTask && firstTask.CurrTaskType == taskType && firstTask.CurrTaskCategory == taskCategory)
                {
                    firstTask.IsCompleted = (firstTask.CountLimit <= int.Parse(metaData[0]));
                }

                if (objTypeMatchesSecondTask && secondTask.CurrTaskType == taskType && secondTask.CurrTaskCategory == taskCategory)
                {
                    secondTask.IsCompleted = (secondTask.CountLimit <= int.Parse(metaData[0]));
                }
                FinalizeTasks();
            }
        }
    }

    public void ResetTasks()
    {
        if(!playerStats.tasksList[playerStats.firstActiveTaskIndex].IsCompleted)
            playerStats.tasksList[playerStats.firstActiveTaskIndex] = firstTask = firstTaskBackup;

        if(!playerStats.tasksList[playerStats.secondActiveTaskIndex].IsCompleted)
            playerStats.tasksList[playerStats.secondActiveTaskIndex] = secondTask = secondTaskBackup;
    }

    public void FinalizeTasks()
    {
        if (firstTask.IsLevelTask && !firstTask.IsCompleted)
            firstTask = firstTaskBackup;
        else
        {
            playerStats.tasksList[playerStats.firstActiveTaskIndex] = firstTask;
            firstTaskBackup = firstTask;
        }
        
        if (secondTask.IsLevelTask && !secondTask.IsCompleted)
            secondTask = secondTaskBackup;
        else
        {
            playerStats.tasksList[playerStats.secondActiveTaskIndex] = secondTask;
            secondTaskBackup = secondTask;
        }
            
    }
    
}
