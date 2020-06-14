using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] GameObject firstTaskGO, secondTaskGO;
    PlayerStatistics playerStats;
    string taskDescription;
    bool firstTimeLoad = true;

    private void UpdateFirstTaskOnScreen()
    {
        int findex = playerStats.firstActiveTaskIndex;
        PlayerStatistics.Task firstTask;
        if (findex != -1)
        {
            firstTask = playerStats.tasksList[findex];
            foreach (Transform child in firstTaskGO.gameObject.transform)
            {
                if (child.gameObject.name == "Text")
                {
                    var tmpTask = child.gameObject.GetComponent<TextMeshProUGUI>();
                    if (firstTask.IsCountable)
                        tmpTask.text = firstTask.TaskDescription + "\n" + firstTask.CurrentCount + " / " + firstTask.CountLimit;
                    else
                        tmpTask.text = firstTask.TaskDescription;
                }

                if (firstTask.IsCompleted)
                {
                    if (child.gameObject.name == "Completed Tag")
                        child.gameObject.SetActive(true);
                    if (child.gameObject.name == "Under Progress Tag")
                        child.gameObject.SetActive(false);
                } else
                {
                    if (child.gameObject.name == "Completed Tag")
                        child.gameObject.SetActive(false);
                    if (child.gameObject.name == "Under Progress Tag")
                        child.gameObject.SetActive(true);
                }

            }
        }
    }

    private void UpdateSecondTaskOnScreen() {
        PlayerStatistics.Task secondTask;
        int sindex = playerStats.secondActiveTaskIndex;
        if (sindex != -1)
        {
            secondTask = playerStats.tasksList[sindex];
            foreach (Transform child in secondTaskGO.transform)
            {
                if (child.gameObject.name == "Text")
                {
                    var tmpTask = child.gameObject.GetComponent<TextMeshProUGUI>();
                    if (secondTask.IsCountable)
                        tmpTask.text = secondTask.TaskDescription + "\n" + secondTask.CurrentCount + " / " + secondTask.CountLimit;
                    else
                        tmpTask.text = secondTask.TaskDescription;
                }

                if (secondTask.IsCompleted)
                {
                    if (child.gameObject.name == "Completed Tag")
                        child.gameObject.SetActive(true);
                    if (child.gameObject.name == "Under Progress Tag")
                        child.gameObject.SetActive(false);
                }
                else
                {
                    if (child.gameObject.name == "Completed Tag")
                        child.gameObject.SetActive(false);
                    if (child.gameObject.name == "Under Progress Tag")
                        child.gameObject.SetActive(true);
                }
            }
        }
    }

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStatistics>();
        PersistentInformation.CurrentChapter = 0;
    }
    
    public void CompleteTask(int index)
    {
        playerStats.TaskCompleted(index);
        if (index == 0)
            UpdateFirstTaskOnScreen();
        else
            UpdateSecondTaskOnScreen();
    }

    void Update()
    {
        if(firstTimeLoad && playerStats.playerStatsLoaded)
        {
            UpdateFirstTaskOnScreen();
            UpdateSecondTaskOnScreen();
            PersistentInformation.CurrentChapter = playerStats.highestChapter;
            firstTimeLoad = false;
        }
    }

    public void LeftArrowClicked()
    {

    }

    public void RightArrowClicked()
    {

    }

}
