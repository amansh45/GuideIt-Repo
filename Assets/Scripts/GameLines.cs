using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLines : MonoBehaviour {

    [SerializeField] GameObject lineDash, sparksPrefab;
    [SerializeField] float screenBorderOffset = 0.2f, spaceBetweenLine = 0.2f, singleDashLength = 0.2f, singleDashWidth = 0.1f, borderZaxis;
    [SerializeField] float borderWidth = 0.025f;
    [SerializeField] Color color = Color.white;
    [SerializeField] GameObject playerPrefab, levelProgressIndicator, levelController, playerTimerUI;
    [SerializeField] Camera mainCamera;
    [SerializeField] float progressIndicatorZAxis = -1.2f, finishLevelAfter = 1.3f;
    [SerializeField] float extraOffsetForCollider = 0.2f;
    [SerializeField] AudioClip levelCompleteSFX;

    TaskHandler taskHandlerClass;
    public float levelCompleted = 0f;

    Vector3 bottomLeft, topLeft, bottomRight, topRight;
    Vector3 firstProgressPosition = new Vector3(0,0,0), secondProgressPosition = new Vector3(0,0,0);
    List<GameObject> gameObjectsForLineRenderer = new List<GameObject>();
    List<LineRenderer> lineRenderersList = new List<LineRenderer>();
    GameObject finishParticle, playSpace, firstProgressInstance, secondProgressInstance;
    Rigidbody2D firstSparkRigidBody, secondSparkRigidBody;
    LevelController levelControllerClass;
    PlayerActions playerActions;
    bool gameRunning = false;
    float levelMaxY = 0, levelMinY = 0, playAreaMinY = 0, playAreaMaxY = 0, playerTimer = 0f, yAxisForBorder;
    TextMeshProUGUI timerLabel;
    ProceduralGeneration pg;
    PlayerStatistics playerStats;

    public Dictionary<string, int> borderLineIndicesMapping = new Dictionary<string, int>()
    {
        {"Left",  0},
        {"Right", 1},
        {"Top", 2},
        {"Bottom", 3}
    };

    private void RenderLine(Vector3 fpoint, Vector3 spoint, int index)
    {
        Vector3 vPos = lineRenderersList[index].transform.position;
        if(borderLineIndicesMapping["Top"] != index)
            lineRenderersList[index].transform.position = new Vector3(vPos.x, vPos.y, borderZaxis);
        else
            lineRenderersList[index].transform.position = new Vector3(vPos.x, vPos.y, (-1) * borderZaxis);
        LineRenderer borderLineRenderer = lineRenderersList[index];
        borderLineRenderer.material = new Material(Shader.Find("Mobile/Particles/Additive"));
        borderLineRenderer.startColor = color;
        borderLineRenderer.endColor = color;
        borderLineRenderer.startWidth = borderWidth;
        borderLineRenderer.endWidth = borderWidth;
        borderLineRenderer.SetPosition(0, fpoint);
        borderLineRenderer.SetPosition(1, spoint);
        borderLineRenderer.useWorldSpace = false;
    }
    

    public void DrawBorderLines(float bottomLeftX, float bottomLeftY, float bottomRightX, float topMargin, float zAxisBorder)
    {
        borderZaxis = zAxisBorder;
        bottomLeft = new Vector3(bottomLeftX, bottomLeftY, borderZaxis);
        bottomRight = new Vector3(bottomRightX, bottomLeftY, borderZaxis);
        
        SpriteRenderer playSpaceRenderer = playSpace.GetComponent<SpriteRenderer>();
        float playSpaceYBound = playSpaceRenderer.bounds.max.y;
        float newYForCollider = playSpaceYBound - topMargin - extraOffsetForCollider + (transform.localScale.y / 2.0f);
        transform.position = new Vector3(transform.position.x, newYForCollider, transform.position.z);
        
        yAxisForBorder = transform.position.y - (transform.localScale.y / 2.0f);
        topLeft = new Vector3(bottomLeftX, yAxisForBorder, borderZaxis);
        topRight = new Vector3(bottomRightX, yAxisForBorder, borderZaxis);

        PersistentInformation.Left = bottomLeft.x;
        PersistentInformation.Right = bottomRight.x;
        PersistentInformation.MarginsSet = true;

        RenderLine(topRight, bottomRight, borderLineIndicesMapping["Right"]);
        RenderLine(bottomLeft, bottomRight, borderLineIndicesMapping["Bottom"]);
        RenderLine(bottomLeft, topLeft, borderLineIndicesMapping["Left"]);
        RenderLine(topLeft, topRight, borderLineIndicesMapping["Top"]);
        levelMaxY = yAxisForBorder;
        levelMinY = bottomLeftY;
    }

    private void Awake()
    {
        finishParticle = transform.GetChild(0).gameObject;
        finishParticle.SetActive(false);
        int currentIndex = gameObject.transform.GetSiblingIndex();
        playSpace = transform.parent.GetChild(currentIndex + 1).gameObject;

        for (int i = 0; i < 4; i++)
        {
            GameObject customGameObject = new GameObject();
            customGameObject.transform.parent = transform.parent;
            customGameObject.name = "Border Line " + i;
            gameObjectsForLineRenderer.Add(customGameObject);
            lineRenderersList.Add(gameObjectsForLineRenderer[i].AddComponent<LineRenderer>());
        }
        firstProgressInstance = Instantiate(levelProgressIndicator, transform.position, transform.rotation);
        secondProgressInstance = Instantiate(levelProgressIndicator, transform.position, transform.rotation);
        Vector3 progressInstanceScale = secondProgressInstance.transform.localScale;
        secondProgressInstance.transform.localScale = new Vector3((-1) * progressInstanceScale.x, progressInstanceScale.y, progressInstanceScale.z);
        firstProgressInstance.transform.name = "Left Progress Indicator";
        secondProgressInstance.transform.name = "Right Progress Indicator";
    }

    private void Start()
    {
        taskHandlerClass = FindObjectOfType<TaskHandler>();
        levelControllerClass = levelController.GetComponent<LevelController>();
        playerActions = playerPrefab.GetComponent<PlayerActions>();
        timerLabel = playerTimerUI.GetComponent<TextMeshProUGUI>();
        pg = FindObjectOfType<ProceduralGeneration>();
        if (pg != null)
            playerTimer = PersistentInformation.timerForInfinitelevel;
        else
            PersistentInformation.timerForInfinitelevel = 0f;
        playerStats = FindObjectOfType<PlayerStatistics>();
    }

    GameObject firstSpark, secondSpark;

    public void SpawnSparks()
    {
        Vector3 cameraBottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 cameraBottomRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0));
        cameraBottomRight.z = topLeft.z;
        cameraBottomLeft.z = topLeft.z;
        firstSpark = Instantiate(sparksPrefab, cameraBottomLeft, transform.rotation);
        secondSpark = Instantiate(sparksPrefab, cameraBottomRight, transform.rotation);
        firstSpark.GetComponent<Spark>().SetParams(true, yAxisForBorder);
        secondSpark.GetComponent<Spark>().SetParams(false, yAxisForBorder);
    }


    void LevelComplete() {
        gameRunning = false;
        Debug.Log("Time Taken: " + playerTimer);
        SpawnSparks();
        finishParticle.SetActive(true);
        Destroy(gameObjectsForLineRenderer[borderLineIndicesMapping["Top"]]);
        float currentVal = topLeft.x + 0.1f;
        while (currentVal <= topRight.x) {
            GameObject dashInstance = Instantiate(lineDash, new Vector3(currentVal, topLeft.y, borderZaxis), transform.rotation);
            dashInstance.transform.parent = transform.parent;
            dashInstance.name = "Dashed Border";
            dashInstance.transform.localScale = new Vector3(singleDashLength, singleDashWidth, 1);
            currentVal += spaceBetweenLine;
        }
        taskHandlerClass.UpdateLevelTaskState(ObjectsDescription.Coin, TaskTypes.CollectAllCoinsInLevel, TaskCategory.ImmediateActionTask, new List<string>() { levelControllerClass.coinsInScene.ToString(), levelControllerClass.currentCoinsAcquired.ToString() });
        //Destroy(playerPrefab);
        AudioSource.PlayClipAtPoint(levelCompleteSFX, mainCamera.transform.position, playerStats.sfxVolume);
        StartCoroutine(ShowLevelCompleteScene());
    }

    IEnumerator ShowLevelCompleteScene()
    {
        yield return new WaitForSeconds(finishLevelAfter);
        levelControllerClass.OnLevelFinished(playerTimer);
    }

    private void Update()
    {
        if(gameRunning && playerPrefab != null && !playerActions.isGamePaused) {
            playerTimer += Time.deltaTime;
            var prevFirstProgressPos = firstProgressInstance.transform.localPosition;
            var prevSecondProgressPos = secondProgressInstance.transform.localPosition;
            float playerY = playerPrefab.transform.position.y, nextProgressY;


            // scaling player wrt the yscale of level to the screen size
            nextProgressY = (((playerY - levelMinY) * (playAreaMaxY - playAreaMinY)) / (levelMaxY - levelMinY)) + playAreaMinY;

            levelCompleted = ((playerY - playAreaMinY) / (levelMaxY - levelMinY));

            if (nextProgressY < playAreaMinY)
                nextProgressY = playAreaMinY;
            else if (nextProgressY > playAreaMaxY)
                nextProgressY = playAreaMaxY;

            firstProgressInstance.transform.localPosition = new Vector3(prevFirstProgressPos.x, nextProgressY, prevFirstProgressPos.z);
            secondProgressInstance.transform.localPosition = new Vector3(prevSecondProgressPos.x, nextProgressY, prevSecondProgressPos.z);

            var minutes = Mathf.FloorToInt(playerTimer / 60);
            var seconds = Mathf.FloorToInt(playerTimer % 60);
            var fraction = (playerTimer * 100) % 99;

            timerLabel.text = string.Format("{0:00} : {1:00}", (minutes * 60) + seconds, fraction);

            if (pg != null)
                PersistentInformation.timerForInfinitelevel = playerTimer;

        }
    }

    public void ShowLevelProgress(float first_x, float second_x, float y, float globalPlayAreaMaxY, GameObject progressParent)
    {
        firstProgressInstance.transform.parent = progressParent.transform;
        secondProgressInstance.transform.parent = progressParent.transform;
        firstProgressInstance.transform.position = new Vector3(first_x, y, progressIndicatorZAxis);
        playAreaMinY = firstProgressInstance.transform.localPosition.y;
        

        // for converting maxY as the localPosition wrt the scrollobject
        secondProgressInstance.transform.position = new Vector3(second_x, globalPlayAreaMaxY, progressIndicatorZAxis);
        playAreaMaxY = secondProgressInstance.transform.localPosition.y;

        secondProgressInstance.transform.position = new Vector3(second_x, y, progressIndicatorZAxis);
        gameRunning = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == ObjectsDescription.Player.ToString()) {
            LevelComplete();
        }
    }
}
