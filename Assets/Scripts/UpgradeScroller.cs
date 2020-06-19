using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            for(int i=0; i<5; i++)
            {
                generateUpgradeItem();
            }
            scrollView.horizontalNormalizedPosition = 0f;
            firstTimeLoad = false;
        }

    }

    void generateUpgradeItem()
    {
        GameObject scrollItemObj = Instantiate(scrollItemPrefab);
        scrollItemObj.transform.parent = scrollContent.transform;
        scrollItemObj.transform.localScale = new Vector3(1, 1, 1);
    }

}
