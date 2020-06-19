using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    PlayerStatistics.Task firstTask, secondTask;
    //public Dictionary<string, Dictionary<string, int>> levelCountTasks = new Dictionary<string, Dictionary<string, int>>();
    bool firstTimeLoad = true;
    PlayerStatistics.Task firstTaskBackup, secondTaskBackup;

    private void Awake()
    {
        int cameraManagerCount = FindObjectsOfType<CameraManager>().Length;
        if (cameraManagerCount > 1)
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

    // real time updation will be done by mainmenuhandler or levelcontroller
    // level needs to be completed for updating the task
    // after level complete levelTask will be completed fully or not at all
    // after level complete gameTask can be completed partially.
    
    public void UpdateLevelTaskState(ObjectsDescription objectType, TaskTypes taskType, TaskCategory taskCategory)
    {
        string objectStr = objectType.ToString();
        string taskTypeStr = taskType.ToString();
        if (taskCategory == TaskCategory.CountingTask)
        {
            /*
            Dictionary<string, int> tasksForObject;
            if (levelCountTasks.ContainsKey(objectStr))
            {
                tasksForObject = levelCountTasks[objectStr];
                if(tasksForObject.ContainsKey(taskTypeStr))
                    levelCountTasks[objectStr][taskTypeStr] += 1;
                else
                    levelCountTasks[objectStr].Add(taskTypeStr, 1);
            } else {
                Dictionary<string, int> task = new Dictionary<string, int>();
                task.Add(taskTypeStr, 1);
                levelCountTasks.Add(objectStr, task);
            }
            */
            if(firstTask.AssociatedWith == objectType && firstTask.CurrTaskType == taskType && firstTask.CurrTaskCategory == taskCategory)
            {
                if (firstTask.CurrentCount < firstTask.CountLimit)
                    firstTask.CurrentCount += 1;
                
                if(firstTask.CurrentCount == firstTask.CountLimit)
                    firstTask.IsCompleted = true;
                Debug.Log(firstTask.TaskDescription +" progress: " + firstTask.CurrentCount);
            }

            if (secondTask.AssociatedWith == objectType && secondTask.CurrTaskType == taskType && secondTask.CurrTaskCategory == taskCategory)
            {
                if (secondTask.CurrentCount < secondTask.CountLimit)
                    secondTask.CurrentCount += 1;
                
                if(secondTask.CurrentCount == secondTask.CountLimit)
                    secondTask.IsCompleted = true;
                Debug.Log(secondTask.TaskDescription + " progress: " + secondTask.CurrentCount);
            }
            
        }
    }

    public void ResetTasks()
    {
        playerStats.tasksList[playerStats.firstActiveTaskIndex] = firstTask = firstTaskBackup;
        playerStats.tasksList[playerStats.secondActiveTaskIndex] = secondTask = secondTaskBackup;
    }

    public void FinalizeTasks()
    {
        if (firstTask.IsLevelTask && !firstTask.IsCompleted)
            firstTask = firstTaskBackup;
        else
            playerStats.tasksList[playerStats.firstActiveTaskIndex] = firstTask;

        if (secondTask.IsLevelTask && !secondTask.IsCompleted)
            secondTask = secondTaskBackup;
        else
            playerStats.tasksList[playerStats.secondActiveTaskIndex] = secondTask;
    }



}
