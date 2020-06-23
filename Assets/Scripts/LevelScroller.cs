﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelScroller : MonoBehaviour
{
    [SerializeField] ScrollRect scrollView;
    [SerializeField] GameObject scrollContent;
    [SerializeField] GameObject scrollItemPrefab;
    int chapterIndex;

    bool firstTimeLoad = true;
    PlayerStatistics playerStats;

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStatistics>();
        chapterIndex = PersistentInformation.CurrentChapter;
    }

    void Update()
    {
        if(!playerStats.playerStatsLoaded)
            playerStats = FindObjectOfType<PlayerStatistics>();

        if (firstTimeLoad && playerStats.playerStatsLoaded)
        {
            Debug.Log("About to load levels.");
            PlayerStatistics.Chapter chapterData = playerStats.chaptersList[chapterIndex];
            List<PlayerStatistics.Level> levelsInChapter = chapterData.LevelsInChapter;
            int numLevels = levelsInChapter.Count;
            for (int i = numLevels-1; i >= 0; i--)
            {
                generateLevelItem(levelsInChapter[i].IsPlayed, levelsInChapter[i].IsPlaying, levelsInChapter[i].IsLocked, levelsInChapter[i].LevelIndex);
            }
            scrollView.verticalNormalizedPosition = 0f;
            firstTimeLoad = false;
        }

    }
    
    void generateLevelItem(bool isPlayed, bool isPlaying, bool isLocked, int levelIndex)
    {
        GameObject scrollItemObj = Instantiate(scrollItemPrefab);
        scrollItemObj.transform.parent = scrollContent.transform;
        scrollItemObj.transform.localScale = new Vector3(1, 1, 1);
        scrollItemObj.name = chapterIndex.ToString() +"."+ levelIndex.ToString();
        if (isLocked)
        {
            foreach (Transform child in scrollItemObj.transform)
            {
                string childName = child.gameObject.name;
                if (childName == "Level Locked")
                    child.gameObject.SetActive(true);
                else
                    child.gameObject.SetActive(false);
            }
        }
        else if (isPlayed)
        {
            foreach (Transform child in scrollItemObj.transform)
            {
                string childName = child.gameObject.name;
                if (childName == "Level Complete")
                    child.gameObject.SetActive(true);
                else
                    child.gameObject.SetActive(false);
            }
        }
        else if (isPlaying)
        {
            foreach (Transform child in scrollItemObj.transform)
            {
                string childName = child.gameObject.name;
                if (childName == "Level Incomplete")
                    child.gameObject.SetActive(true);
                else
                    child.gameObject.SetActive(false);
            }
        }
    }

}