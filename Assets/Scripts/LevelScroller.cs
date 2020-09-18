using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelScroller : MonoBehaviour
{
    [SerializeField] ScrollRect scrollView;
    [SerializeField] GameObject scrollContent;
    [SerializeField] GameObject scrollItemPrefab;
    [SerializeField] GameObject coinsTextGO;
    int chapterIndex;

    bool firstTimeLoad = true;
    PlayerStatistics playerStats;
    TextMeshProUGUI coinsText;
    AdMob adMob;

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStatistics>();
        chapterIndex = PersistentInformation.CurrentChapter;
        coinsText = coinsTextGO.GetComponent<TextMeshProUGUI>();
        adMob = FindObjectOfType<AdMob>();
        adMob.RequestBanner();
    }

    void Update()
    {
        if(!playerStats.playerStatsLoaded)
            playerStats = FindObjectOfType<PlayerStatistics>();

        if (firstTimeLoad && playerStats.playerStatsLoaded)
        {
            coinsText.text = playerStats.playerCoins.ToString();
            PlayerStatistics.Chapter chapterData = playerStats.chaptersList[chapterIndex];
            List<PlayerStatistics.Level> levelsInChapter = chapterData.LevelsInChapter;
            int numLevels = levelsInChapter.Count;
            for (int i = numLevels-1; i >= 0; i--)
            {
                generateLevelItem(levelsInChapter[i].IsPlayed, levelsInChapter[i].IsPlaying, levelsInChapter[i].IsLocked, 
                    levelsInChapter[i].LevelIndex, levelsInChapter[i].PersonalBestTime, levelsInChapter[i].CoinsAcquiredInLevel, levelsInChapter[i].CoinsInLevel);
            }
            scrollView.verticalNormalizedPosition = 0f;
            firstTimeLoad = false;
        }

    }
    
    void generateLevelItem(bool isPlayed, bool isPlaying, bool isLocked, int levelIndex, float personalBestTime, int coinsAcquired, int totalCoinsInLevel)
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

                foreach (Transform grandChild in child)
                {
                    if (grandChild.name == "Left Level Num" || grandChild.name == "Right Level Num")
                    {
                        TextMeshProUGUI textField = grandChild.gameObject.GetComponent<TextMeshProUGUI>();
                        textField.text = (levelIndex + 1).ToString();
                    }
                    else if(grandChild.name == "Personal Best Time")
                    {
                        var minutes = Mathf.FloorToInt(personalBestTime / 60);
                        var seconds = Mathf.FloorToInt(personalBestTime % 60);
                        var fraction = (personalBestTime * 100) % 99;
                        string timeStr = string.Format("{0:00}:{1:00}", (minutes * 60) + seconds, fraction) + " s";
                        TextMeshProUGUI textField = grandChild.gameObject.GetComponent<TextMeshProUGUI>();
                        textField.text = "Best run: " + timeStr;
                    } else if(grandChild.name == "Coins Acquired Data")
                    {
                        TextMeshProUGUI textField = grandChild.gameObject.GetComponent<TextMeshProUGUI>();
                        textField.text = "Juice: " + coinsAcquired.ToString() + " / " + totalCoinsInLevel.ToString();
                    }
                }
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

                foreach (Transform grandChild in child)
                {
                    if (grandChild.name == "Left Level Num" || grandChild.name == "Right Level Num")
                    {
                        TextMeshProUGUI textFeild = grandChild.gameObject.GetComponent<TextMeshProUGUI>();
                        textFeild.text = (levelIndex + 1).ToString();
                    }
                }
            }
        }
    }

}
