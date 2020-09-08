using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData {

    public List<PlayerStatistics.Task> tasksList = new List<PlayerStatistics.Task>();
    public List<PlayerStatistics.Chapter> chaptersList = new List<PlayerStatistics.Chapter>();
    public List<PlayerStatistics.Upgrade> upgradesList = new List<PlayerStatistics.Upgrade>();
    public int currentChapter, firstActiveTaskIndex, secondActiveTaskIndex, tasksCompleted, highestChapter, highestLevel, playerCoins;

    public void PopulatePlayerData(List<PlayerStatistics.Task> tasksList, List<PlayerStatistics.Chapter> chaptersList, List<PlayerStatistics.Upgrade> upgradesList,
        int firstActiveTaskIndex, int secondActiveTaskIndex, int tasksCompleted, int highestChapter, int highestLevel,  int playerCoins)
    {
        currentChapter = PersistentInformation.CurrentChapter;
        this.firstActiveTaskIndex = firstActiveTaskIndex;
        this.secondActiveTaskIndex = secondActiveTaskIndex;
        this.tasksCompleted = tasksCompleted;
        this.tasksList = tasksList;
        this.chaptersList = chaptersList;
        this.upgradesList = upgradesList;
        this.highestChapter = highestChapter;
        this.highestLevel = highestLevel;
        this.playerCoins = playerCoins;
    }

}
