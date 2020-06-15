using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] GameObject firstTaskGO, secondTaskGO, chapterNameGO, loadIcon;
    PlayerStatistics playerStats;
    string taskDescription;
    bool firstTimeLoad = true;
    TextMeshProUGUI chapterName;
    int numChapters;

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
        chapterName = chapterNameGO.GetComponent<TextMeshProUGUI>();
    }
    
    public void CompleteTask(int index)
    {
        playerStats.TaskCompleted(index);
        if (index == 0)
            UpdateFirstTaskOnScreen();
        else
            UpdateSecondTaskOnScreen();
    }

    private void UpdateIcons() {
        if (playerStats.chaptersList[PersistentInformation.CurrentChapter].IsLocked)
        {
            foreach(Transform child in loadIcon.transform)
            {
                string childName = child.gameObject.name;
                if (childName == "Load")
                    child.gameObject.SetActive(false);
                else if (childName == "Locked")
                    child.gameObject.SetActive(true);
                else if (childName == "Outer Circle")
                    child.gameObject.GetComponent<Animator>().enabled = false;
            }
        } else
        {
            foreach (Transform child in loadIcon.transform)
            {
                string childName = child.gameObject.name;
                if (childName == "Load")
                    child.gameObject.SetActive(true);
                else if (childName == "Locked")
                    child.gameObject.SetActive(false);
                else if (childName == "Outer Circle")
                    child.gameObject.GetComponent<Animator>().enabled = true;
            }
        }
    }

    void Update()
    {
        if(firstTimeLoad && playerStats.playerStatsLoaded)
        {
            UpdateFirstTaskOnScreen();
            UpdateSecondTaskOnScreen();
            numChapters = playerStats.chaptersList.Count;
            PersistentInformation.CurrentChapter = playerStats.highestChapter;
            chapterName.text = playerStats.chaptersList[PersistentInformation.CurrentChapter].ChapterName;
            UpdateIcons();
            firstTimeLoad = false;
        }
    }

    public void LeftArrowClicked()
    {
        if(PersistentInformation.CurrentChapter > 0)
        {
            PersistentInformation.CurrentChapter = PersistentInformation.CurrentChapter - 1;
            chapterName.text = playerStats.chaptersList[PersistentInformation.CurrentChapter].ChapterName;
            UpdateIcons();
        }
    }

    public void RightArrowClicked()
    {
        if (PersistentInformation.CurrentChapter < numChapters - 1)
        {
            PersistentInformation.CurrentChapter = PersistentInformation.CurrentChapter + 1;
            chapterName.text = playerStats.chaptersList[PersistentInformation.CurrentChapter].ChapterName;
            UpdateIcons();
        }
    }

    public void LoadChapter()
    {
        if(!playerStats.chaptersList[PersistentInformation.CurrentChapter].IsLocked)
            SceneManager.LoadScene("Chapter");
    }


}
