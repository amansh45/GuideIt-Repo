using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// types of tasks.
public enum TaskTypes
{
    NearMiss,
    Collect,
    Destroy,
    UpdateSkin,
    Follow,
    Rest,
    ScreenClick,
    NoHit,
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
    //public Dictionary<string, Dictionary<string, int>> levelCountTasks = new Dictionary<string, Dictionary<string, int>>();
    bool firstTimeLoad = true;
    PlayerStatistics.Task firstTaskBackup, secondTaskBackup;

    private void Awake()
    {
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
        string taskAssociatedWith, taskType, taskCategory;

        if (findex != -1)
        {
            firstTask = playerStats.tasksList[findex];
            firstTaskBackup = firstTask;
            taskAssociatedWith = firstTask.AssociatedWith.ToString();
            taskType = firstTask.CurrTaskType.ToString();
            taskCategory = firstTask.CurrTaskCategory.ToString();
        }

        if (sindex != -1)
        {
            secondTask = playerStats.tasksList[sindex];
            secondTaskBackup = secondTask;
            taskAssociatedWith = firstTask.AssociatedWith.ToString();
            taskType = firstTask.CurrTaskType.ToString();
            taskCategory = firstTask.CurrTaskCategory.ToString();
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

    private bool IsLevelEligibleToExecuteTask(string sceneName)
    {
        string[] levelChapterIndex = sceneName.Split('.');
        PlayerStatistics.Level levelData = playerStats.chaptersList[int.Parse(levelChapterIndex[0])].LevelsInChapter[int.Parse(levelChapterIndex[1])];
        return !levelData.IsPlayed;
    }

    private bool UpdateCameFromLevel(string sceneName)
    {
        return sceneName.Contains(".");
    }

    // real time updation will be done by mainmenuhandler or levelcontroller
    // level needs to be completed for updating the task
    // after level complete levelTask will be completed fully or not at all
    // after level complete gameTask can be completed partially.
    public void UpdateLevelTaskState(ObjectsDescription objectType, TaskTypes taskType, TaskCategory taskCategory)
    {
        Scene scene = SceneManager.GetActiveScene();
        string sceneName = scene.name;
        if (UpdateCameFromLevel(sceneName) && !IsLevelEligibleToExecuteTask(sceneName))
            return;

        string objectStr = objectType.ToString();
        string taskTypeStr = taskType.ToString();
        if (taskCategory == TaskCategory.CountingTask)
        {
            if(firstTask.AssociatedWith == objectType && firstTask.CurrTaskType == taskType && firstTask.CurrTaskCategory == taskCategory)
            {
                if (firstTask.CurrentCount < firstTask.CountLimit)
                    firstTask.CurrentCount += 1;
                
                if(firstTask.CurrentCount == firstTask.CountLimit)
                    firstTask.IsCompleted = true;
            }

            if (secondTask.AssociatedWith == objectType && secondTask.CurrTaskType == taskType && secondTask.CurrTaskCategory == taskCategory)
            {
                if (secondTask.CurrentCount < secondTask.CountLimit)
                    secondTask.CurrentCount += 1;
                
                if(secondTask.CurrentCount == secondTask.CountLimit)
                    secondTask.IsCompleted = true;
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
