﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
    public struct Task
    {
        public bool IsCompleted, IsCountable;
        public int TaskIndex, TaskCompletionAward, CountLimit, CurrentCount;
        public ObjectsDescription AssociatedWith;
        public string TaskDescription;

        public Task(bool isCompleted, ObjectsDescription associatedWith, string taskDescription, int taskIndex, int taskCompletionAward, bool isCountable)
        {
            IsCompleted = isCompleted;
            AssociatedWith = associatedWith;
            TaskIndex = taskIndex;
            TaskCompletionAward = taskCompletionAward;
            TaskDescription = taskDescription;
            IsCountable = isCountable;
            CountLimit = 0;
            CurrentCount = 0;
        }

        public Task(bool isCompleted, ObjectsDescription associatedWith, string taskDescription, int taskIndex, int taskCompletionAward, bool isCountable, int countLimit)
        {
            IsCompleted = isCompleted;
            AssociatedWith = associatedWith;
            TaskIndex = taskIndex;
            TaskCompletionAward = taskCompletionAward;
            TaskDescription = taskDescription;
            IsCountable = isCountable;
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

    public List<Task> tasksList = new List<Task>();
    public List<Chapter> chaptersList = new List<Chapter>();
    public List<Upgrade> upgradesList = new List<Upgrade>();
    public int firstActiveTaskIndex = 0, secondActiveTaskIndex = 1, tasksCompleted = 0, totalTasks = 0, highestChapter = 0, highestLevel = 0;
    public bool playerStatsLoaded = false;
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
                level = new Level(0, 0, i, true, false, false, "test");
            else
                level = new Level(0, 0, i, false, false, true, "test");
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
        /*
        Task task = new Task(false, ObjectsDescription.Coin, "Collect 10 coins", 0, 20, true, 10);
        tasksList.Add(task);
        task = new Task(true, ObjectsDescription.Player, "Complete Level in one go", 2, 50, false);
        tasksList.Add(task);
        task = new Task(false, ObjectsDescription.Coin, "Collect 40 coins", 1, 80, true, 40);
        tasksList.Add(task);
        task = new Task(false, ObjectsDescription.EnemyLauncher, "Destroy 8 enemy cannons", 3, 100, true, 8);
        tasksList.Add(task);
        */

        Task task = new Task(true, ObjectsDescription.EnemyLauncher, "1", 3, 100, true, 8);
        tasksList.Add(task);
        task = new Task(true, ObjectsDescription.EnemyLauncher, "2", 3, 100, true, 8);
        tasksList.Add(task);
        task = new Task(true, ObjectsDescription.EnemyLauncher, "3", 3, 100, true, 8);
        tasksList.Add(task);
        task = new Task(true, ObjectsDescription.EnemyLauncher, "4", 3, 100, true, 8);
        tasksList.Add(task);
        task = new Task(true, ObjectsDescription.EnemyLauncher, "5", 3, 100, true, 8);
        tasksList.Add(task);
        task = new Task(true, ObjectsDescription.EnemyLauncher, "6", 3, 100, true, 8);
        tasksList.Add(task);
        task = new Task(true, ObjectsDescription.EnemyLauncher, "7", 3, 100, true, 8);
        tasksList.Add(task);
        task = new Task(true, ObjectsDescription.EnemyLauncher, "8", 3, 100, true, 8);
        tasksList.Add(task);
        task = new Task(true, ObjectsDescription.EnemyLauncher, "9", 3, 100, true, 8);
        tasksList.Add(task);
        task = new Task(true, ObjectsDescription.EnemyLauncher, "10", 3, 100, true, 8);
        tasksList.Add(task);
        task = new Task(true, ObjectsDescription.EnemyLauncher, "11", 3, 100, true, 8);
        tasksList.Add(task);
        task = new Task(true, ObjectsDescription.EnemyLauncher, "12", 3, 100, true, 8);
        tasksList.Add(task);
        task = new Task(true, ObjectsDescription.EnemyLauncher, "13", 3, 100, true, 8);
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
