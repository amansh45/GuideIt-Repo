﻿using System;
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

    public struct CustomColor
    {
        public Color FirstColor, SecondColor, ThirdColor, FourthColor;

        public CustomColor(Color firstColor, Color secondColor, Color thirdColor, Color fourthColor)
        {
            FirstColor = firstColor;
            SecondColor = secondColor;
            ThirdColor = thirdColor;
            FourthColor = fourthColor;
        }
    }

    public struct SkinColorStuff
    {
        public int CoinCost;
        public float MoneyCost;
        public bool IsActive;
        public bool IsUnlocked;

        public SkinColorStuff(int coinCost, float moneyCost, bool isActive, bool isUnlocked)
        {
            CoinCost = coinCost;
            MoneyCost = moneyCost;
            IsActive = isActive;
            IsUnlocked = isUnlocked;
        }
    }

    public struct Upgrade
    {
        public int CoinCost;
        public float MoneyCost;
        public bool IsBoughtByCoin, IsBoughtByMoney, IsUnlocked, IsActive;
        public ObjectsDescription ApplicableOn;
        public GameObject UpgradeParticle;
        public SkinColors ParticlesColor;
        public SkinCategory UpgradeCategory; 
        public string UpgradeName;
        public Dictionary<string, SkinColorStuff> ColorStuff;

        public Upgrade(int coinCost, float moneyCost, bool isBoughtByCoin, bool isBoughtByMoney, bool isUnlocked, bool isActive, ObjectsDescription applicableOn, 
            SkinCategory upgradeCategory, GameObject upgradeParticle, SkinColors particlesColor, string upgradeName, Dictionary<string, SkinColorStuff> colorStuff)
        {
            CoinCost = coinCost;
            MoneyCost = moneyCost;
            IsBoughtByCoin = isBoughtByCoin;
            IsBoughtByMoney = isBoughtByMoney;
            IsUnlocked = isUnlocked;
            IsActive = isActive;
            ApplicableOn = applicableOn;
            UpgradeParticle = upgradeParticle;
            ParticlesColor = particlesColor;
            UpgradeName = upgradeName;
            UpgradeCategory = upgradeCategory;
            ColorStuff = colorStuff;
        }
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
    public List<int> activeUpgrades = new List<int>();
    public int firstActiveTaskIndex = 0, secondActiveTaskIndex = 1, tasksCompleted = 0, totalTasks = 0, highestChapter = 0, highestLevel = 0, playerCoins;
    public bool playerStatsLoaded = false;
    public LevelCompletionData levelCompletionData;
    public Dictionary<string, CustomColor> colorsData = new Dictionary<string, CustomColor>();
    [SerializeField] List<int> levelsInChapter = new List<int>();
    [SerializeField] GameObject playerBasic, launcherBasic, bulletBasic, playerModerate, launcherModerate, bulletModerate, playerAdvance, launcherAdvance, bulletAdvance;

    private void Awake()
    {
        int playerStatsCount = FindObjectsOfType<PlayerStatistics>().Length;
        if (playerStatsCount > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    private List<Level> AddLevels(int chapterIndex)
    {
        List<Level> levels = new List<Level>();
        int numLevels = levelsInChapter[chapterIndex];
        for (int i = 0; i < numLevels; i++)
        {
            Level level;
            if (chapterIndex == 0 && i == 0)
                level = new Level(int.MaxValue, 0, i, true, false, false, "test");
            else
                level = new Level(int.MaxValue, 0, i, false, false, true, "test");
            levels.Add(level);
        }
        return levels;
    }

    private void AddChapters()
    {
        for (int i = 0; i < levelsInChapter.Count; i++)
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

    public string IntegerToRoman(int number)
    {
        String[] roman = new String[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
        int[] decimals = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };

        string romanValue = String.Empty;

        for (int i = 0; i < 13; i++)
        {
            while (number >= decimals[i])
            {
                number -= decimals[i];
                romanValue += roman[i];
            }
        }

        return romanValue;
    }

    private void AddTasks()
    {

        Task task = new Task(false, true, ObjectsDescription.Coin, "Collect 5 coins in any level", 0, 20, TaskTypes.Collect, TaskCategory.CountingTask);
        tasksList.Add(task);
        task = new Task(false, false, ObjectsDescription.EnemyLauncher, "Destroy 4 enemy Cannons", 1, 100, TaskTypes.Destroy, TaskCategory.CountingTask);
        tasksList.Add(task);
        task = new Task(false, true, ObjectsDescription.Coin, "Collect 10 coins in any level", 2, 20, TaskTypes.Collect, TaskCategory.CountingTask);
        tasksList.Add(task);
        task = new Task(false, true, ObjectsDescription.Player, "Complete Level in one go", 3, 50, TaskTypes.NoHit, TaskCategory.CountingTask);
        tasksList.Add(task);

        totalTasks = tasksList.Count;
    }

    private Dictionary<string, SkinColorStuff> CostForColor()
    {
        Dictionary<string, SkinColorStuff> mDict = new Dictionary<string, SkinColorStuff>();

        SkinColorStuff stuff;
        
        stuff = new SkinColorStuff(100, 10f, true, true);
        mDict.Add(SkinColors.Yellow.ToString(), stuff);
        stuff = new SkinColorStuff(130, 12f, false, false);
        mDict.Add(SkinColors.Blue.ToString(), stuff);
        stuff = new SkinColorStuff(180, 15f, false, false);
        mDict.Add(SkinColors.Red.ToString(), stuff);
        stuff = new SkinColorStuff(200, 20f, false, false);
        mDict.Add(SkinColors.Purple.ToString(), stuff);
        
        return mDict;
    }

    private void AddUpgrades()
    {
        
        Upgrade upgrade = new Upgrade(0, 0f, false, false, true, true, ObjectsDescription.Player, SkinCategory.PlayerBasic, playerBasic, SkinColors.Yellow, IntegerToRoman(1), CostForColor());
        upgradesList.Add(upgrade);

        upgrade = new Upgrade(500, 5f, false, false, false, false, ObjectsDescription.Player, SkinCategory.PlayerModerate, playerModerate, SkinColors.Yellow, IntegerToRoman(2), CostForColor());
        upgradesList.Add(upgrade);

        upgrade = new Upgrade(1000, 10f, false, false, false, false, ObjectsDescription.Player, SkinCategory.PlayerAdvance, playerAdvance, SkinColors.Yellow, IntegerToRoman(3), CostForColor());
        upgradesList.Add(upgrade);

        upgrade = new Upgrade(0, 0f, false, false, true, true, ObjectsDescription.PlayerLauncher, SkinCategory.LauncherBasic, launcherBasic, SkinColors.Yellow, IntegerToRoman(4), CostForColor());
        upgradesList.Add(upgrade);

        upgrade = new Upgrade(500, 5f, false, false, false, false, ObjectsDescription.PlayerLauncher, SkinCategory.LauncherModerate, launcherModerate, SkinColors.Yellow, IntegerToRoman(5), CostForColor());
        upgradesList.Add(upgrade);

        upgrade = new Upgrade(1000, 10f, false, false, false, false, ObjectsDescription.PlayerLauncher, SkinCategory.LauncherAdvance, launcherAdvance, SkinColors.Yellow, IntegerToRoman(6), CostForColor());
        upgradesList.Add(upgrade);

        upgrade = new Upgrade(0, 0f, false, false, true, true, ObjectsDescription.PlayerProjectile, SkinCategory.BulletBasic, bulletBasic, SkinColors.Yellow, IntegerToRoman(7), CostForColor());
        upgradesList.Add(upgrade);

        upgrade = new Upgrade(500, 5f, false, false, false, false, ObjectsDescription.PlayerProjectile, SkinCategory.BulletModerate, bulletModerate, SkinColors.Yellow, IntegerToRoman(8), CostForColor());
        upgradesList.Add(upgrade);

        upgrade = new Upgrade(1000, 10f, false, false, false, false, ObjectsDescription.PlayerProjectile, SkinCategory.BulletAdvance, bulletAdvance, SkinColors.Yellow, IntegerToRoman(9), CostForColor());
        upgradesList.Add(upgrade);
    }

    public Color HexToRGB(string code)
    {
        Color testColor;
        ColorUtility.TryParseHtmlString(code, out testColor);
        return testColor;
    }

    private void DumpColors()
    {
        CustomColor mcolor = new CustomColor(HexToRGB("#FF7D1E"), HexToRGB("#FFDF5D"), HexToRGB("#FFE981"), HexToRGB("#FFBA00")); ;
        colorsData.Add(SkinColors.Yellow.ToString(), mcolor);
        mcolor = new CustomColor(HexToRGB("#1E31FF"), HexToRGB("#5DCDFF"), HexToRGB("#66E5FF"), HexToRGB("#001BFF"));
        colorsData.Add(SkinColors.Blue.ToString(), mcolor);
        mcolor = new CustomColor(HexToRGB("#FF0F00"), HexToRGB("#FF6C5E"), HexToRGB("#FFA27F"), HexToRGB("#FF1500"));
        colorsData.Add(SkinColors.Red.ToString(), mcolor);
        mcolor = new CustomColor(HexToRGB("#FF00EC"), HexToRGB("#FF5ED4"), HexToRGB("#FF90EC"), HexToRGB("#F700FF"));
        colorsData.Add(SkinColors.Purple.ToString(), mcolor);
    }

    private void Start()
    {
        PersistentInformation.CurrentChapter = 0;
        playerCoins = 1000;
        AddTasks();
        AddChapters();
        DumpColors();
        AddUpgrades();
        playerStatsLoaded = true;
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