using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] GameObject firstTaskGO, secondTaskGO, chapterNameGO, loadIcon, upgradesIconGO, collectedCoinsPrefab;
    [SerializeField] float spawingOffset = 0.5f;
    [SerializeField] List<GameObject> chapterIcons;

    PlayerStatistics playerStats;
    string taskDescription;
    bool firstTimeLoad = true;
    TextMeshProUGUI chapterName;
    TaskHandler taskHandlerClass;
    int numChapters;
    Vector3 firstTasksCoinPos, secondTasksCoinPos, destinationPos;
    static System.Random random = new System.Random();
    List<Color[]> chapterIconColors = new List<Color[]>();
    List<Image> chapterIconsImage = new List<Image>();

    private void UpdateFirstTaskOnScreen(bool isTaskCompleted)
    {
        int findex = playerStats.firstActiveTaskIndex;
        PlayerStatistics.Task firstTask;
        if (findex != -1)
        {
            firstTask = playerStats.tasksList[findex];

            if(isTaskCompleted)
                playerStats.playerCoins += firstTask.TaskCompletionAward;

            foreach (Transform child in firstTaskGO.gameObject.transform)
            {
                if (child.gameObject.name == "Text")
                {
                    var tmpTask = child.gameObject.GetComponent<TextMeshProUGUI>();
                    if (firstTask.CurrTaskCategory == TaskCategory.CountingTask)
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

    private void UpdateSecondTaskOnScreen(bool isTaskCompleted) {
        PlayerStatistics.Task secondTask;
        int sindex = playerStats.secondActiveTaskIndex;
        if (sindex != -1)
        {
            secondTask = playerStats.tasksList[sindex];

            if(isTaskCompleted)
                playerStats.playerCoins += secondTask.TaskCompletionAward;

            foreach (Transform child in secondTaskGO.transform)
            {
                if (child.gameObject.name == "Text")
                {
                    var tmpTask = child.gameObject.GetComponent<TextMeshProUGUI>();
                    if (secondTask.CurrTaskCategory == TaskCategory.CountingTask)
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
        chapterName = chapterNameGO.GetComponent<TextMeshProUGUI>();
        taskHandlerClass = FindObjectOfType<TaskHandler>();
        foreach (Transform child in secondTaskGO.transform)
        {
            if (child.gameObject.name == "Completed Tag" || child.gameObject.name == "Under Progress Tag")
            {
                secondTasksCoinPos = Camera.main.ScreenToWorldPoint(child.position);
                break;
            }
        }

        foreach (Transform child in firstTaskGO.transform)
        {
            if (child.gameObject.name == "Completed Tag" || child.gameObject.name == "Under Progress Tag")
            {
                firstTasksCoinPos = Camera.main.ScreenToWorldPoint(child.position);
                break;
            }
        }

        destinationPos = Camera.main.ScreenToWorldPoint(upgradesIconGO.transform.position);
        destinationPos.z = 0f;
        
        for(int i=0;i<chapterIcons.Count;i++)
        {
            var x = chapterIcons[i].GetComponent<Image>();
            chapterIconsImage.Add(x);
        }

    }

    public float GetRandomNumber(float minimum, float maximum)
    {
        return ((float) random.NextDouble() * (maximum - minimum) + minimum);
    }

    IEnumerator spawnCoins(Vector3 pos)
    {
        yield return new WaitForSeconds(GetRandomNumber(0, 0.6f));
        GameObject instance = Instantiate(collectedCoinsPrefab, pos, transform.rotation);
        instance.GetComponent<CollectedCoins>().destinationPos = destinationPos;
    }

    private void SpawnCoins(Vector3 spawningPos)
    {
        for(int i=0;i<20;i++)
        {
            float[] randVal = { GetRandomNumber(-1f*spawingOffset, spawingOffset), GetRandomNumber(-1f * spawingOffset, spawingOffset) };
            Vector3 pos = new Vector3(spawningPos.x + randVal[0], spawningPos.y + randVal[1], 0f);
            StartCoroutine(spawnCoins(pos));
        }
    }

    public void CompleteTask(int index)
    {
        playerStats.TaskCompleted(index);
        if (index == 0)
        {
            SpawnCoins(firstTasksCoinPos);
            UpdateFirstTaskOnScreen(true);
        }
        else
        {
            SpawnCoins(secondTasksCoinPos);
            UpdateSecondTaskOnScreen(true);
        }
        taskHandlerClass.UpdateTaskPointers();
        
    }

    private void UpdateIcons() {
        int currChapter = PersistentInformation.CurrentChapter;
        if (playerStats.chaptersList[currChapter].IsLocked)
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

        int k = 0;
        for(int i=0;i<=currChapter;i++)
        {
            chapterIconsImage[i].color = chapterIconColors[currChapter][k++];
        }

        for(int i=currChapter+1;i<chapterIcons.Count;i++)
        {
            chapterIconsImage[i].color = Color.white;
        }

    }

    private void InitializeChapterIconColors()
    {
        Color[] fColorsList = { playerStats.HexToRGB("#FFFF62") };
        chapterIconColors.Add(fColorsList);
        Color[] sColorsList = { playerStats.HexToRGB("#A564FF"), playerStats.HexToRGB("#8832FF") };
        chapterIconColors.Add(sColorsList);
        Color[] tColorsList = { playerStats.HexToRGB("#FFC886"), playerStats.HexToRGB("#FFB051"), playerStats.HexToRGB("#FFA232") };
        chapterIconColors.Add(tColorsList);
        Color[] foColorsList = { playerStats.HexToRGB("#BBF1FF"), playerStats.HexToRGB("#75E2FF"), playerStats.HexToRGB("#51DAFF"), playerStats.HexToRGB("#2DD2FF") };
        chapterIconColors.Add(foColorsList);
    }

    void Update()
    {
        if (!playerStats.playerStatsLoaded)
            playerStats = FindObjectOfType<PlayerStatistics>();

        if (firstTimeLoad && playerStats.playerStatsLoaded)
        {
            InitializeChapterIconColors();
            UpdateFirstTaskOnScreen(false);
            UpdateSecondTaskOnScreen(false);
            numChapters = playerStats.chaptersList.Count;
            PersistentInformation.CurrentChapter = playerStats.highestChapter;
            chapterName.text = playerStats.chaptersList[PersistentInformation.CurrentChapter].ChapterName;
            UpdateIcons();
            firstTimeLoad = false;
        }

        if (taskHandlerClass == null)
            taskHandlerClass = FindObjectOfType<TaskHandler>();

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
