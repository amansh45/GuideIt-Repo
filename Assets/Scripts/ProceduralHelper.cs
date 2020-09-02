using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralHelper : MonoBehaviour
{
    [SerializeField] Objects enemyObjects;
    [System.Serializable]
    public struct Objects
    {
        public GameObject square;
        public GameObject bigBox;
        public GameObject sphere;
        public GameObject blade;
        public GameObject box;
        public GameObject launcher;
        public GameObject grinder;
        public GameObject hPlatform;
        public GameObject saw;
        public GameObject stillCannon;
        public GameObject upCannon;
        public GameObject stillPlatform;
        public GameObject sphereSupportingPlatform;
    }

    [SerializeField] GameObject playSpaceGO, playerPrefab, slowmotionGO;

    Slowmotion slowmoClass;
    public Dictionary<string, GameObject> objectsDict = new Dictionary<string, GameObject>();
    PlayerStatistics playerStats;

    public void CreateDictionary()
    {
        objectsDict.Add("square", enemyObjects.square); //
        objectsDict.Add("blade", enemyObjects.blade); //
        objectsDict.Add("box", enemyObjects.box); // 
        objectsDict.Add("launcher", enemyObjects.launcher); //
        objectsDict.Add("stillCannon", enemyObjects.stillCannon); //
        objectsDict.Add("upCannon", enemyObjects.upCannon); //
        objectsDict.Add("bigBox", enemyObjects.bigBox); //
        objectsDict.Add("grinder", enemyObjects.grinder); //
        objectsDict.Add("sphere", enemyObjects.sphere); //
        objectsDict.Add("hPlatform", enemyObjects.hPlatform); //
        objectsDict.Add("saw", enemyObjects.saw); //
        objectsDict.Add("stillPlatform", enemyObjects.stillPlatform); //
        objectsDict.Add("sphereSupportingPlatform", enemyObjects.sphereSupportingPlatform); //
    }

    public void UpdatePlaceObjectScriptParams(GameObject go, bool isRotating, bool placeWrtCorners, bool isCoinOrPlayer, bool scalingRequired, float dynamicWidthForScaling)
    {
        PlaceObjects placeObjects = go.GetComponent<PlaceObjects>();
        placeObjects.isRotating = isRotating;
        placeObjects.placeWrtCornors = placeWrtCorners;
        placeObjects.scalingRequired = scalingRequired;
        placeObjects.isCoinOrPlayer = isCoinOrPlayer;
        placeObjects.dynamicWidthForScaling = dynamicWidthForScaling;
    }

    public void LoadLevelFromStats()
    {
        playerStats = FindObjectOfType<PlayerStatistics>();
        CreateDictionary();
        Dictionary<string, List<GameObject>> gameObjectsDictForSlowmo = new Dictionary<string, List<GameObject>>();

        List<PlayerStatistics.ObjectsData> levelObjectsData = playerStats.listOfObjects;
        int numObjects = levelObjectsData.Count;
        for(int i=0;i<numObjects;i++)
        {
            PlayerStatistics.ObjectsData objectData = levelObjectsData[i];
            GameObject goInstance = (objectData.ObjectType == "hPlatform:") ? objectsDict["hPlatform"] : objectsDict[objectData.ObjectType];
            GameObject go = Instantiate(goInstance, objectData.ObjectPosition, Quaternion.Euler(objectData.ObjectRotation));

            if(gameObjectsDictForSlowmo.ContainsKey(objectData.ObjectType))
            {
                gameObjectsDictForSlowmo[objectData.ObjectType].Add(go);
            }
            else
                gameObjectsDictForSlowmo.Add(objectData.ObjectType, new List<GameObject> { go });


            //Debug.Log("Length is: " + objectData.ObjectType + gameObjectsDictForSlowmo[objectData.ObjectType].Count);

            // : is used to denote when count for horizontalplatform is 3
            if (objectData.ObjectType == "hPlatform:")
            {
                foreach (Transform child in go.transform)
                {
                    if (child.gameObject.name == "LR Horizontal Platform (2)" || child.gameObject.name == "LR Horizontal Platform (3)")
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }

            if(objectData.ObjectType == "bigBox")
            {
                GameObject fBox = go.transform.GetChild(0).gameObject;
                GameObject sBox = go.transform.GetChild(1).gameObject;

                PlayerStatistics.BigBoxData boxChildParams = (PlayerStatistics.BigBoxData) objectData.BigBoxParams;

                Debug.Log("First box Rotation Vector is: " + boxChildParams.FBoxRotation);
                Debug.Log("Second box Rotation Vector is: " + boxChildParams.SBoxRotation);

                fBox.transform.localPosition = boxChildParams.FBoxPos;
                fBox.transform.localScale = boxChildParams.FBoxScale;
                fBox.transform.rotation = Quaternion.Euler(boxChildParams.FBoxRotation);
                sBox.transform.localPosition = boxChildParams.SBoxPos;
                sBox.transform.localScale = boxChildParams.SBoxScale;
                sBox.transform.rotation = Quaternion.Euler(boxChildParams.SBoxRotation);
            }


            if (objectData.ObjectType == "sphere")
            {
                go.GetComponent<InitiateFall>().playerPrefab = playerPrefab;
            }

            if (objectData.ObjectScale != null)
                go.transform.localScale = (Vector3) objectData.ObjectScale;
            if (objectData.IsAnimationChangeRequired)
            {
                Animator goAnimator = go.GetComponentInChildren<Animator>();
                goAnimator.runtimeAnimatorController = objectData.AnimatorController;
            }
            
            if(objectData.ScriptParams != null)
            {
                PlayerStatistics.PlaceObjectScriptParams scriptParams = (PlayerStatistics.PlaceObjectScriptParams) objectData.ScriptParams;
                UpdatePlaceObjectScriptParams(go, scriptParams.IsRotating, scriptParams.PlaceWrtCorners, scriptParams.IsCoinOrPlayer, scriptParams.ScalingRequired, scriptParams.DynamicWidthForScaling);
            }

            if(objectData.LauncherScriptParams != null)
            {
                PlayerStatistics.EnemyLauncherScriptParams launcherScriptParams = (PlayerStatistics.EnemyLauncherScriptParams) objectData.LauncherScriptParams;
                EnemyLauncher cannon = go.GetComponentInChildren<EnemyLauncher>();
                cannon.isBlinking = launcherScriptParams.IsBlinking;
                cannon.aimAtPlayer = launcherScriptParams.AimAtPlayer;
                cannon.playerPrefab = playerPrefab;
            }

        }
        SetPlaySpaceAtRuntime(levelObjectsData[numObjects - 1].ObjectPosition.y + 5f);

        AddObjectsForSlowMotion(gameObjectsDictForSlowmo);

    }

    public void AddObjectsForSlowMotion(Dictionary<string, List<GameObject>> objectDictForSlowmo)
    {
        foreach (KeyValuePair<string, List<GameObject>> obj in objectDictForSlowmo) {

            List<GameObject> slowmoCandidates = obj.Value;
            int size = slowmoCandidates.Count;
            Debug.Log("Number of " + obj.Key + ": " + size);

            
            for(int i=0;i<size;i++)
            {
                if (obj.Key == "box" || obj.Key == "blade" || obj.Key == "square" || obj.Key == "upCannon")
                {
                    slowmoClass.gameEntities.Add(slowmoCandidates[i].transform.GetChild(0).gameObject);
                }
                else if (obj.Key == "launcher")
                {
                    slowmoClass.gameEntities.Add(slowmoCandidates[i].transform.GetChild(1).gameObject);
                }
                else if (obj.Key == "grinder")
                {
                    slowmoClass.gameEntities.Add(slowmoCandidates[i].transform.GetChild(0).GetChild(1).gameObject);
                    slowmoClass.gameEntities.Add(slowmoCandidates[i].transform.GetChild(1).GetChild(1).gameObject);
                }
                else if (obj.Key == "hPlatform" || obj.Key == "stillCannon")
                    slowmoClass.gameEntities.Add(slowmoCandidates[i]);
            }
            
        }
    }

    public void SetPlaySpaceAtRuntime(float yAxis)
    {
        var rend = playSpaceGO.GetComponent<SpriteRenderer>();

        float initialBound = rend.bounds.min.y;
        float minBound;
        float maxBound = rend.bounds.max.y;
        Vector3 prevPlaySpaceScale = playSpaceGO.transform.localScale;

        while (maxBound < yAxis)
        {
            playSpaceGO.transform.localScale = new Vector3(prevPlaySpaceScale.x, prevPlaySpaceScale.y + 0.1f, prevPlaySpaceScale.z);
            minBound = rend.bounds.min.y;

            while (minBound < initialBound)
            {
                playSpaceGO.transform.position = new Vector3(playSpaceGO.transform.position.x, playSpaceGO.transform.position.y + 0.1f, playSpaceGO.transform.position.z);
                minBound = rend.bounds.min.y;
            }
            maxBound = rend.bounds.max.y;
            prevPlaySpaceScale = playSpaceGO.transform.localScale;
        }

    }

    private void Awake()
    { 
        slowmoClass = slowmotionGO.GetComponent<Slowmotion>();
    }

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStatistics>();
    }
}
