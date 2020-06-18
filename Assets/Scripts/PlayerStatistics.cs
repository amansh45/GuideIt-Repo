using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    public struct Task
    {
        public bool IsCompleted, IsLevelTask;
        public int TaskIndex, TaskCompletionAward, CurrentCount;
        public float CountLimit;
        public ObjectsDescription AssociatedWith;
        public TaskTypes CurrTaskType;
        public TaskCategory CurrTaskCategory;
        public string TaskDescription;

        public Task(bool isCompleted, bool isLevelTask, ObjectsDescription associatedWith, string taskDescription, int taskIndex, int taskCompletionAward, TaskTypes taskType, TaskCategory taskCategory)
        {
            IsCompleted = isCompleted;
            IsLevelTask = isLevelTask;
            AssociatedWith = associatedWith;
            TaskIndex = taskIndex;
            TaskCompletionAward = taskCompletionAward;
            TaskDescription = taskDescription;
            CurrTaskCategory = taskCategory;
            CurrTaskType = taskType;
            CountLimit = 0;
            CurrentCount = 0;
        }

        public Task(bool isCompleted, bool isLevelTask, ObjectsDescription associatedWith, string taskDescription, int taskIndex, int taskCompletionAward, TaskTypes taskType, TaskCategory taskCategory, float countLimit)
        {
            IsCompleted = isCompleted;
            IsLevelTask = isLevelTask;
            AssociatedWith = associatedWith;
            TaskIndex = taskIndex;
            TaskCompletionAward = taskCompletionAward;
            TaskDescription = taskDescription;
            CurrTaskCategory = taskCategory;
            CurrTaskType = taskType;
            CountLimit = countLimit;
            CurrentCount = 0;
        }


    }

    public struct Upgrade
    {
        string upgradeType;
        int coinCost;
        int moneyCost;
        bool isBoughtByCoin;
        bool isBoughtByMoney;
        bool isUnlocked;
        bool isActive;
        string applicableOn;
        List<GameObject> particles;
    }

    public struct Level
    {
        public float PersonalBestTime;
        public int CoinsAcquiredInLevel, LevelIndex;
        public bool IsPlaying, IsPlayed, IsLocked;
        public string LevelType;

        public Level(float personalBestTime, int coinsAcquiredInLevel, int levelIndex, bool isPlaying, bool isPlayed, bool isLocked, string levelType)
        {
            PersonalBestTime = personalBestTime;
            CoinsAcquiredInLevel = coinsAcquiredInLevel;
            LevelIndex = levelIndex;
            IsPlaying = isPlaying;
            IsPlayed = isPlayed;
            IsLocked = isLocked;
            LevelType = levelType;
        }
    }

    public struct Chapter
    {
        public string ChapterName;
        public List<Level> LevelsInChapter;
        public bool IsLocked;

        public Chapter(string chapterName, List<Level> levelsInChapter, bool isLocked)
        {
            ChapterName = chapterName;
            LevelsInChapter = levelsInChapter;
            IsLocked = isLocked;
        }
    }

    public struct LevelCompletionData
    {
        public int ChapterIndex, LevelIndex, TotalCoins, CoinsCollected;
        public float RecentTime, BestTime;

        public LevelCompletionData(int chapterIndex, int levelIndex, int totalCoins, int coinsCollected, float recentTime, float bestTime)
        {
            ChapterIndex = chapterIndex;
            LevelIndex = levelIndex;
            TotalCoins = totalCoins;
            CoinsCollected = coinsCollected;
            RecentTime = recentTime;
            BestTime = bestTime;
        }

    }

    public List<Task> tasksList = new List<Task>();
    public List<Chapter> chaptersList = new List<Chapter>();
    public List<Upgrade> upgradesList = new List<Upgrade>();
    public int firstActiveTaskIndex = 0, secondActiveTaskIndex = 1, tasksCompleted = 0, totalTasks = 0, highestChapter = 0, highestLevel = 0, playerCoins;
    public bool playerStatsLoaded = false;
    public LevelCompletionData levelCompletionData;
    [SerializeField] List<int> levelsInChapter = new List<int>();

    private void Awake()
    {
        int playerStatsCount = FindObjectsOfType<PlayerStatistics>().Length;
        if (playerStatsCount > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        PersistentInformation.CurrentChapter = 0;
        playerCoins = 0;
        AddTasks();
        AddChapters();
        playerStatsLoaded = true;
    }

    private List<Level> AddLevels(int chapterIndex)
    {
        List<Level> levels = new List<Level>();
        int numLevels = levelsInChapter[chapterIndex];
        for(int i=0;i<numLevels;i++)
        {
            Level level;
            if(chapterIndex == 0 && i == 0)
                level = new Level(int.MaxValue, 0, i, true, false, false, "test");
            else
                level = new Level(int.MaxValue, 0, i, false, false, true, "test");
            levels.Add(level);
        }
        return levels;
    }

    private void AddChapters()
    {
        for(int i=0;i<levelsInChapter.Count;i++)
        {
            string chapterName = "Chapter " + (i + 1).ToString();
            List<Level> levels = AddLevels(i);
            Chapter chapter;
            if (i == 0)
                chapter = new Chapter(chapterName, levels, false);
            else
                chapter = new Chapter(chapterName, levels, true);
            chaptersList.Add(chapter);
        }
    }

    private void AddTasks()
    {
        
        Task task = new Task(false, false, ObjectsDescription.Coin, "Collect 10 coins in any level", 0, 20, TaskTypes.Collect, TaskCategory.CountingTask, 10f);
        tasksList.Add(task);
        task = new Task(false, true, ObjectsDescription.Player, "Complete Level in one go", 1, 50, TaskTypes.NoHit, TaskCategory.CountingTask, 1f);
        tasksList.Add(task);
        task = new Task(false, true, ObjectsDescription.Coin, "Collect 10 coins in any level", 2, 20, TaskTypes.Collect, TaskCategory.CountingTask, 40f);
        tasksList.Add(task);
        task = new Task(false, true, ObjectsDescription.EnemyLauncher, "Destroy 18 enemy Cannons", 3, 100, TaskTypes.Destroy, TaskCategory.CountingTask, 18f);
        tasksList.Add(task);
        
        totalTasks = tasksList.Count;
    }

    public void TaskCompleted(int taskIndex)
    {
        tasksCompleted += 1;
        
        if (taskIndex == 0 && firstActiveTaskIndex != -1)
        {
            Task data = tasksList[firstActiveTaskIndex];
            data.IsCompleted = true;
            tasksList[firstActiveTaskIndex] = data;
            if (secondActiveTaskIndex != -1)
            {
                firstActiveTaskIndex = Mathf.Max(secondActiveTaskIndex, firstActiveTaskIndex) + 1;
                if (firstActiveTaskIndex == totalTasks)
                {
                    firstActiveTaskIndex = -1;
                    return;
                }
            }
            else
                firstActiveTaskIndex = -1;
        } else if (taskIndex == 1 && secondActiveTaskIndex != -1)
        {
            Task data = tasksList[secondActiveTaskIndex];
            data.IsCompleted = true;
            tasksList[secondActiveTaskIndex] = data;
            if (firstActiveTaskIndex != -1)
            {
                secondActiveTaskIndex = Mathf.Max(secondActiveTaskIndex, firstActiveTaskIndex) + 1;
                if (secondActiveTaskIndex == totalTasks)
                {
                    secondActiveTaskIndex = -1;
                    return;
                }
            }
            else
                secondActiveTaskIndex = -1;
        }
    }

}
