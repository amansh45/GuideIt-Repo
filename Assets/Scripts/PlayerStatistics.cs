using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class PlayerStatistics : MonoBehaviour
{
    public struct PlaceObjectScriptParams
    {
        public bool IsRotating;
        public bool PlaceWrtCorners;
        public bool IsCoinOrPlayer;
        public bool ScalingRequired;
        public float DynamicWidthForScaling;

        public PlaceObjectScriptParams(bool isRotating, bool placeWrtCorners, bool isCoinOrPlayer, bool scalingRequired, float dynamicWidthForScaling)
        {
            IsRotating = isRotating;
            PlaceWrtCorners = placeWrtCorners;
            IsCoinOrPlayer = isCoinOrPlayer;
            ScalingRequired = scalingRequired;
            DynamicWidthForScaling = dynamicWidthForScaling;
        }

    }

    public struct BigBoxData
    {
        public Vector3 FBoxPos;
        public Vector3 SBoxPos;
        public Vector3 FBoxRotation;
        public Vector3 SBoxRotation;
        public Vector3 FBoxScale;
        public Vector3 SBoxScale;

        public BigBoxData(Vector3 fBoxPos, Vector3 sBoxPos, Vector3 fBoxRotation, Vector3 sBoxRotation, Vector3 fBoxScale, Vector3 sBoxScale)
        {
            FBoxPos = fBoxPos;
            SBoxPos = sBoxPos;
            FBoxRotation = fBoxRotation;
            SBoxRotation = sBoxRotation;
            FBoxScale = fBoxScale;
            SBoxScale = sBoxScale;
        }

    }

    public struct EnemyLauncherScriptParams
    {
        public bool AimAtPlayer;
        public bool IsBlinking;

        public EnemyLauncherScriptParams(bool aimAtPlayer, bool isBlinking)
        {
            AimAtPlayer = aimAtPlayer;
            IsBlinking = isBlinking;
        }
    }

    public struct ObjectsData
    {
        public string ObjectType;
        public Vector3 ObjectPosition;
        public Vector3? ObjectScale;
        public Vector3 ObjectRotation;
        public PlaceObjectScriptParams? ScriptParams;
        public bool IsAnimationChangeRequired;
        public EnemyLauncherScriptParams? LauncherScriptParams;
        public BigBoxData? BigBoxParams;
        public RuntimeAnimatorController AnimatorController;

        public ObjectsData(string objectType, Vector3 objectPosition, Vector3? objectScale, Vector3 objectRotation, 
            PlaceObjectScriptParams? scriptParams, bool isAnimationChangeRequired, 
            RuntimeAnimatorController animationController, EnemyLauncherScriptParams? launcherScriptParams, BigBoxData? bigBoxParams)
        {
            ObjectType = objectType;
            ObjectPosition = objectPosition;
            ObjectScale = objectScale;
            ObjectRotation = objectRotation;
            ScriptParams = scriptParams;
            IsAnimationChangeRequired = isAnimationChangeRequired;
            AnimatorController = animationController;
            LauncherScriptParams = launcherScriptParams;
            BigBoxParams = bigBoxParams;
        }
    }

    public List<ObjectsData> listOfObjects = new List<ObjectsData>();
    public bool prevProceduralLevelFailed = false;

    public void AddObjectToSaveList(ObjectsData objectsData)
    {
        listOfObjects.Add(objectsData);
    }


    public struct Task
    {
        public bool IsCompleted, IsLevelTask;
        public int TaskIndex, TaskCompletionAward, CurrentCount;
        public float CountLimit;
        public List<ObjectsDescription> AssociatedWith;
        public TaskTypes CurrTaskType;
        public TaskCategory CurrTaskCategory;
        public string TaskDescription;

        public Task(bool isCompleted, bool isLevelTask, List<ObjectsDescription> associatedWith, string taskDescription, int taskIndex, int taskCompletionAward, TaskTypes taskType, TaskCategory taskCategory)
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

        public Task(bool isCompleted, bool isLevelTask, List<ObjectsDescription> associatedWith, string taskDescription, int taskIndex, int taskCompletionAward, TaskTypes taskType, TaskCategory taskCategory, float countLimit)
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
        public SkinColors ParticlesColor;
        public SkinCategory UpgradeCategory; 
        public string UpgradeName;
        public Dictionary<string, SkinColorStuff> ColorStuff;

        public Upgrade(int coinCost, float moneyCost, bool isBoughtByCoin, bool isBoughtByMoney, bool isUnlocked, bool isActive, ObjectsDescription applicableOn, 
            SkinCategory upgradeCategory, SkinColors particlesColor, string upgradeName, Dictionary<string, SkinColorStuff> colorStuff)
        {
            CoinCost = coinCost;
            MoneyCost = moneyCost;
            IsBoughtByCoin = isBoughtByCoin;
            IsBoughtByMoney = isBoughtByMoney;
            IsUnlocked = isUnlocked;
            IsActive = isActive;
            ApplicableOn = applicableOn;
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
        public int numTimesLevelFailed;
        public int numTimesNearMiss;

        public Level(float personalBestTime, int coinsAcquiredInLevel, int levelIndex, bool isPlaying, bool isPlayed, bool isLocked, string levelType)
        {
            PersonalBestTime = personalBestTime;
            CoinsAcquiredInLevel = coinsAcquiredInLevel;
            LevelIndex = levelIndex;
            IsPlaying = isPlaying;
            IsPlayed = isPlayed;
            IsLocked = isLocked;
            LevelType = levelType;
            numTimesLevelFailed = 0;
            numTimesNearMiss = 0;
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
    public Dictionary<string, CustomColor> colorsData = new Dictionary<string, CustomColor>();
    [SerializeField] List<int> levelsInChapter = new List<int>();

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


    // two objects can't be combined together while assignment of tasks in a game.
    private void AddTasks()
    {
        int index = 0;
        Task task = new Task(false, false, new List<ObjectsDescription>() { ObjectsDescription.Player }, "Follow on Instagram", index++, 200, TaskTypes.Follow, TaskCategory.ImmediateActionTask);
        tasksList.Add(task);
        task = new Task(false, true, new List<ObjectsDescription>() { ObjectsDescription.Box }, "Near miss a box thrice in any level", index++, 100, TaskTypes.NearMiss, TaskCategory.CountingTask, 3);
        tasksList.Add(task);
        task = new Task(false, false, new List<ObjectsDescription>() { ObjectsDescription.BigFallingSphere }, "Push 2 sphere out of the way by shooting at them", index++, 50, TaskTypes.Collide, TaskCategory.CountingTask, 2);
        tasksList.Add(task);
        task = new Task(false, true, new List<ObjectsDescription>() { ObjectsDescription.EnemyProjectile }, "Destroy 2 misslles by shooting at it", index++, 50, TaskTypes.Destroy, TaskCategory.CountingTask, 2);
        tasksList.Add(task);
        task = new Task(false, true, new List<ObjectsDescription>() { ObjectsDescription.Player }, "Complete a level with zero near miss", index++, 5, TaskTypes.NoNearMiss, TaskCategory.ImmediateActionTask);
        tasksList.Add(task);
        task = new Task(false, true, new List<ObjectsDescription>() { ObjectsDescription.Coin }, "Collect 1 coin in any level", index++, 5, TaskTypes.Collect, TaskCategory.CountingTask, 1);
        tasksList.Add(task);
        task = new Task(false, true, new List<ObjectsDescription>() { ObjectsDescription.Player }, "Complete a level without getting hit", index++, 5, TaskTypes.NoHit, TaskCategory.ImmediateActionTask);
        tasksList.Add(task);
        task = new Task(false, true, new List<ObjectsDescription>() { ObjectsDescription.Player }, "Complete any level without making ball to hover", index++, 5, TaskTypes.Hover, TaskCategory.ImmediateActionTask);
        tasksList.Add(task);
        task = new Task(false, true, new List<ObjectsDescription>() { ObjectsDescription.Box }, "Near miss a box thrice in any level", index++, 100, TaskTypes.NearMiss, TaskCategory.CountingTask, 3);
        tasksList.Add(task);
        task = new Task(false, true, new List<ObjectsDescription>() { ObjectsDescription.Coin }, "Collect all coins in a level", index++, 15, TaskTypes.CollectAllCoinsInLevel, TaskCategory.ImmediateActionTask);
        tasksList.Add(task);
        task = new Task(false, true, new List<ObjectsDescription>() { ObjectsDescription.EnemyLauncher }, "Destroy 1 enemy Cannons in any level", index++, 100, TaskTypes.Destroy, TaskCategory.CountingTask, 1);
        tasksList.Add(task);
        task = new Task(false, false, new List<ObjectsDescription>() { ObjectsDescription.Coin }, "Collect 8 coins", index++, 50, TaskTypes.Collect, TaskCategory.CountingTask, 10);
        tasksList.Add(task);
        task = new Task(false, false, new List<ObjectsDescription>() { ObjectsDescription.EnemyLauncher }, "Destroy 2 enemy Cannons", index++, 100, TaskTypes.Destroy, TaskCategory.CountingTask, 2);
        tasksList.Add(task);
        task = new Task(false, false, new List<ObjectsDescription>() { ObjectsDescription.Coin }, "Collect 4 coins", index++, 20, TaskTypes.Collect, TaskCategory.CountingTask, 4);
        tasksList.Add(task);
        task = new Task(false, true, new List<ObjectsDescription>() { ObjectsDescription.EnemyLauncher }, "Destroy 3 enemy Cannons in any level", index++, 100, TaskTypes.Destroy, TaskCategory.CountingTask, 3);
        tasksList.Add(task);
        task = new Task(false, false, new List<ObjectsDescription>() { ObjectsDescription.Coin }, "Collect 20 coins in total", index++, 20, TaskTypes.Collect, TaskCategory.CountingTask, 20);
        tasksList.Add(task);
        task = new Task(false, true, new List<ObjectsDescription>() { ObjectsDescription.Player }, "Complete Level in one go", index++, 50, TaskTypes.NoHit, TaskCategory.CountingTask, 1);
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
        Upgrade upgrade = new Upgrade(0, 0f, false, false, true, true, ObjectsDescription.Player, SkinCategory.PlayerBasic, SkinColors.Yellow, IntegerToRoman(1), CostForColor());
        upgradesList.Add(upgrade);

        upgrade = new Upgrade(500, 5f, false, false, false, false, ObjectsDescription.Player, SkinCategory.PlayerModerate, SkinColors.Yellow, IntegerToRoman(2), CostForColor());
        upgradesList.Add(upgrade);
        
        /*
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
        */
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

    /*
 * Updating the color of the skin
 * 
 */

    private void UpdatePlayerSkinColor(GameObject mat, PlayerStatistics.Upgrade currUpgrade, GameObject player)
    {
        PlayerStatistics.CustomColor mcolor = colorsData[currUpgrade.ParticlesColor.ToString()];
        player.GetComponent<SpriteRenderer>().color = mcolor.ThirdColor;

        PlayerActions pa = player.GetComponent<PlayerActions>();

        if(pa != null)
            pa.SetColorForPathLines(mcolor.ThirdColor);


        foreach (Transform child in mat.transform)
        {
            ParticleSystem ps = child.GetComponent<ParticleSystem>();
            if (child.transform.name == "Player Trail" || child.transform.name == "Player Particles")
            {
                //Debug.Log("Updating "+ child.transform.name + "to: " + currUpgrade.ParticlesColor.ToString());
                var col = ps.colorOverLifetime;
                col.enabled = true;
                Gradient grad = new Gradient();
                grad.SetKeys(new GradientColorKey[] { new GradientColorKey(mcolor.FirstColor, 0.0f), new GradientColorKey(mcolor.SecondColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
                col.color = grad;
            }
            else if (child.transform.name == "Player Glow")
            {
                ParticleSystem.MinMaxGradient grad = new ParticleSystem.MinMaxGradient(mcolor.ThirdColor, mcolor.FourthColor);
                var main = ps.main;
                main.startColor = grad;
            }
        }
    }

    public void UpdateColorOfSkin(PlayerStatistics.Upgrade currentUpgrade, GameObject targetGO)
    {

        if (currentUpgrade.ApplicableOn == ObjectsDescription.Player)
        {
            if (currentUpgrade.UpgradeCategory == SkinCategory.PlayerBasic)
            {
                GameObject mat = targetGO.transform.GetChild(0).gameObject;
                Debug.Log("Inside basic, name of upgrade: " + mat.name);
                UpdatePlayerSkinColor(mat, currentUpgrade, targetGO);
            }
            else if (currentUpgrade.UpgradeCategory == SkinCategory.PlayerModerate)
            {
                GameObject mat = targetGO.transform.GetChild(0).gameObject;
                Debug.Log("Inside moderate, name of upgrade: " + mat.name);
                UpdatePlayerSkinColor(mat, currentUpgrade, targetGO);
            }
        }
        else if (currentUpgrade.ApplicableOn == ObjectsDescription.PlayerLauncher)
        {

        }
        else if (currentUpgrade.ApplicableOn == ObjectsDescription.PlayerProjectile)
        {

        }
    }

    /*
     * End
     */

}
