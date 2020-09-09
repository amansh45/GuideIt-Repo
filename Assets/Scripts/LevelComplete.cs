using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] float borderScreenOffset = 1f, borderWidth = 0.025f, finishBackgroundZAxis = -1.3f;
    [SerializeField] Color color = Color.white;
    [SerializeField] GameObject coinTextGO, levelIndexGO, prevBestTimeGO, currentTimeGO; 

    Vector3 bottomLeft, topRight, bottomRight, topLeft;
    PlayerStatistics playerStats;
    bool firstTimeLoad = true;
    
    public void RenderLine(Vector3 fpoint, Vector3 spoint)
    {
        GameObject borderLineGO = new GameObject();
        borderLineGO.transform.parent = transform;
        borderLineGO.transform.position = transform.position;
        borderLineGO.transform.name = "Border Line";
        LineRenderer borderLineRenderer = borderLineGO.AddComponent<LineRenderer>();
        borderLineRenderer.material = new Material(Shader.Find("Mobile/Particles/Additive"));
        borderLineRenderer.startColor = color;
        borderLineRenderer.endColor = color;
        borderLineRenderer.startWidth = borderWidth;
        borderLineRenderer.endWidth = borderWidth;
        borderLineRenderer.SetPosition(0, fpoint);
        borderLineRenderer.SetPosition(1, spoint);
        borderLineRenderer.useWorldSpace = false;
    }
    
    public void CreateBorders()
    {
        Vector3 bLeft, bRight, tLeft, tRight;
        bLeft = new Vector3(bottomLeft.x + borderScreenOffset, bottomLeft.y + borderScreenOffset, finishBackgroundZAxis);
        bRight = new Vector3(bottomRight.x - borderScreenOffset, bottomRight.y + borderScreenOffset, finishBackgroundZAxis);
        tLeft = new Vector3(topLeft.x + borderScreenOffset, topLeft.y - borderScreenOffset, finishBackgroundZAxis);
        tRight = new Vector3(topRight.x - borderScreenOffset, topRight.y - borderScreenOffset, finishBackgroundZAxis);
        RenderLine(bLeft, bRight);
        RenderLine(tLeft, tRight);
        RenderLine(bLeft, tLeft);
        RenderLine(bRight, tRight);
    }

    void Start()
    {
        bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        bottomRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0));
        topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        topLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, 0));
        float cameraWidth = bottomRight.x - bottomLeft.x;
        float cameraHeight = topRight.y - bottomLeft.y;
        playerStats = FindObjectOfType<PlayerStatistics>();

        CreateBorders();
        
    }

    private void Update()
    {
        if (!playerStats.playerStatsLoaded)
            playerStats = FindObjectOfType<PlayerStatistics>();

        if(firstTimeLoad && playerStats.playerStatsLoaded)
        {
            PlayerStatistics.LevelCompletionData completedLevel = playerStats.levelCompletionData;

            coinTextGO.GetComponent<TextMeshProUGUI>().text = playerStats.playerCoins.ToString();
            levelIndexGO.GetComponent<TextMeshProUGUI>().text = completedLevel.LevelIndex.ToString();

            var minutes = Mathf.FloorToInt(completedLevel.RecentTime / 60);
            var seconds = Mathf.FloorToInt(completedLevel.RecentTime % 60);
            var fraction = (completedLevel.RecentTime * 100) % 99;
            currentTimeGO.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00} : {1:00}", (minutes * 60) + seconds, fraction)+"s";


            if (completedLevel.BestTime == int.MaxValue)
                prevBestTimeGO.GetComponent<TextMeshProUGUI>().text = "Best Time:  --";
            else
            {
                minutes = Mathf.FloorToInt(completedLevel.BestTime / 60);
                seconds = Mathf.FloorToInt(completedLevel.BestTime % 60);
                fraction = (completedLevel.BestTime * 100) % 99;
                prevBestTimeGO.GetComponent<TextMeshProUGUI>().text = "Best Time: " + string.Format("{0:00} : {1:00}", (minutes * 60) + seconds, fraction)+"s";
            }
                

            firstTimeLoad = false;
        }
    }

    public void LoadNextLevel()
    {
        string levelName = PersistentInformation.LevelIdentifier;
        string[] levelIdentity = levelName.Split('.');
        int currentChapterIndex = int.Parse(levelIdentity[0]);
        int currentLevelIndex = int.Parse(levelIdentity[1]);

        if (currentLevelIndex == playerStats.chaptersList[currentChapterIndex].LevelsInChapter.Count - 1)
        {
            if (currentChapterIndex == playerStats.chaptersList.Count - 1)
            {

            }
            else
            {
                SceneManager.LoadScene((currentChapterIndex + 1).ToString() + ".0");
            }

        }
        else
        {
            SceneManager.LoadScene(currentChapterIndex.ToString() + "." + (currentLevelIndex + 1).ToString());
        }
    }

    public void RetryCurrentLevel()
    {
        string levelName = PersistentInformation.LevelIdentifier;
        SceneManager.LoadScene(levelName);
    }

}
