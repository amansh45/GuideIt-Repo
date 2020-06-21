using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeScroller : MonoBehaviour
{
    [SerializeField] ScrollRect scrollView;
    [SerializeField] GameObject scrollContent;
    [SerializeField] GameObject scrollItemPrefab;


    bool firstTimeLoad = true;
    PlayerStatistics playerStats;

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStatistics>();
    }

    void Update()
    {
        if (!playerStats.playerStatsLoaded)
            playerStats = FindObjectOfType<PlayerStatistics>();

        if (firstTimeLoad && playerStats.playerStatsLoaded)
        {
            int numUpgrades = playerStats.upgradesList.Count;
            for(int i=0; i<numUpgrades; i++)
            {
                generateUpgradeItem(i);
            }
            scrollView.horizontalNormalizedPosition = 0f;
            firstTimeLoad = false;
        }

    }

    void generateUpgradeItem(int index)
    {
        PlayerStatistics.Upgrade upgradeData = playerStats.upgradesList[index];
        GameObject scrollItemObj = Instantiate(scrollItemPrefab);
        scrollItemObj.transform.parent = scrollContent.transform;
        scrollItemObj.transform.localScale = new Vector3(1, 1, 1);

        foreach(Transform child in scrollItemObj.transform)
        {
            if(upgradeData.IsUnlocked)
            {
                if (child.name == "Lock Image")
                    child.gameObject.SetActive(false);
                else if (child.name == "Unlock Image")
                    child.gameObject.SetActive(true);
            } else
            {
                if (child.name == "Lock Image")
                    child.gameObject.SetActive(true);
                else if (child.name == "Unlock Image")
                    child.gameObject.SetActive(false);
            }

            if(child.transform.name == "Upgrade Name")
            {
                TextMeshProUGUI upgradeName = child.gameObject.GetComponent<TextMeshProUGUI>();
                upgradeName.text = upgradeData.UpgradeName;
            }
        }

        scrollItemObj.transform.name = index.ToString();
    }

}
