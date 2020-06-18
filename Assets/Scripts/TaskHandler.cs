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
    PlayerStatistics playerStats;
    PlayerStatistics.Task firstTask, secondTask;

    /*
        launcher -> {
                        nearMiss ->  CountingTask,
                        destroy  ->  CountingTask,
                    }

    */

    public Dictionary<string, Dictionary<string, int>> levelCountTasks = new Dictionary<string, Dictionary<string, int>>();
    bool firstTimeLoad = true; 
    
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


    private void Update()
    {
        if(firstTimeLoad && playerStats.isActiveAndEnabled)
        {
            int findex = playerStats.firstActiveTaskIndex, sindex = playerStats.secondActiveTaskIndex;
            string taskAssociatedWith, taskType, taskCategory;

            if (findex != -1)
            {
                firstTask = playerStats.tasksList[findex];
                taskAssociatedWith = firstTask.AssociatedWith.ToString();
                taskType = firstTask.CurrTaskType.ToString();
                taskCategory = firstTask.CurrTaskCategory.ToString();
            }
            
            if (sindex != -1)
            {
                secondTask = playerStats.tasksList[sindex];
                taskAssociatedWith = firstTask.AssociatedWith.ToString();
                taskType = firstTask.CurrTaskType.ToString();
                taskCategory = firstTask.CurrTaskCategory.ToString();
            }

            firstTimeLoad = false;
        }
    }

    public void UpdateLevelTaskState(ObjectsDescription objectType, TaskTypes taskType, TaskCategory taskCategory)
    {
        string objectStr = objectType.ToString();
        string taskTypeStr = taskType.ToString();
        if (taskCategory == TaskCategory.CountingTask)
        {
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
        }
    }

}
