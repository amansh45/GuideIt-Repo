using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class ProceduralGeneration : MonoBehaviour
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
    
    [SerializeField] GameObject coinsPrefab, playerGO;
    [SerializeField] RuntimeAnimatorController snappingAnimationController, rotateAnimationController, l2rAnimationController, stillCannonAnimationController, blinkingCannonAnimationController;
    [SerializeField] GameObject playSpaceGO;

    public bool hasLevelGenerationCompleted = false;

    Dictionary<string, GameObject> objectsDict = new Dictionary<string, GameObject>();
    float screenWidth, screenCenterXPoint;

    void CreateDictionary()
    {
        objectsDict.Add("square", enemyObjects.square); //
        objectsDict.Add("bigBox", enemyObjects.bigBox); //
        objectsDict.Add("sphere", enemyObjects.sphere); //
        objectsDict.Add("blade", enemyObjects.blade); //
        objectsDict.Add("box", enemyObjects.box); // 
        objectsDict.Add("launcher", enemyObjects.launcher); //
        objectsDict.Add("grinder", enemyObjects.grinder); //
        objectsDict.Add("hPlatform", enemyObjects.hPlatform); //
        objectsDict.Add("saw", enemyObjects.saw); 
        objectsDict.Add("stillCannon", enemyObjects.stillCannon); //
        objectsDict.Add("upCannon", enemyObjects.upCannon); //
        objectsDict.Add("stillPlatform", enemyObjects.stillPlatform); //
        objectsDict.Add("sphereSupportingPlatform", enemyObjects.sphereSupportingPlatform); //
    }


    public int GetRandomWeightedIndex(float[] weights)
    {
        float weightSum = 0f;
        int elementCount = weights.Length;
        for (int i = 0; i < elementCount; ++i)
        {
            weightSum += weights[i];
        }

        int index = 0;
        int lastIndex = elementCount - 1;
        float randomWeight = UnityEngine.Random.Range(0, weightSum);
        while (index < lastIndex)
        {
            if (randomWeight < weights[index])
            {
                return index;
            }

            randomWeight -= weights[index++];
        }

        return index;
    }


    void UpdatePlaceObjectScriptParams(GameObject go, bool isRotating, bool placeWrtCorners, bool isCoinOrPlayer, bool scalingRequired, float dynamicWidthForScaling)
    {
        PlaceObjects placeObjects = go.GetComponent<PlaceObjects>();
        placeObjects.isRotating = isRotating;
        placeObjects.placeWrtCornors = placeWrtCorners;
        placeObjects.scalingRequired = scalingRequired;
        placeObjects.isCoinOrPlayer = isCoinOrPlayer;
        placeObjects.dynamicWidthForScaling = dynamicWidthForScaling;
    }

    void CreateL2RAnimation(int animatorIndex, int count, string objectType, float yDistanceBetweenObjects, float spawningYCoordinate)
    {
        GameObject first = objectsDict[objectType];
        GameObject second = objectsDict[objectType];
        GameObject third = objectsDict[objectType];
        GameObject fourth = objectsDict[objectType];

        Vector3 fPos = new Vector3((screenCenterXPoint / 2f) - 0.5f, spawningYCoordinate, 0);
        GameObject firstGO = Instantiate(first, fPos, Quaternion.Euler(new Vector3(0, 0, 0)));
        Animator firstAnimator = firstGO.GetComponentInChildren<Animator>();
        UpdatePlaceObjectScriptParams(firstGO, false, true, false, true, 0);

        if (count == 1)
            return;

        Vector3 sPos = new Vector3((screenCenterXPoint / 2f) + 0.5f, spawningYCoordinate + yDistanceBetweenObjects, 0);
        GameObject secondGO = Instantiate(second, sPos, Quaternion.Euler(new Vector3(0, 180, 360)));
        Animator secondAnimator = secondGO.GetComponentInChildren<Animator>();
        UpdatePlaceObjectScriptParams(secondGO, false, true, false, true, 0);

        firstAnimator.runtimeAnimatorController = (animatorIndex == 0) ? l2rAnimationController : snappingAnimationController;
        secondAnimator.runtimeAnimatorController = (animatorIndex == 0) ? l2rAnimationController : snappingAnimationController;

        if(count  == 3 || count == 4)
        {
            Vector3 tPos = new Vector3((screenCenterXPoint / 2f) - 0.5f, spawningYCoordinate + 2 * yDistanceBetweenObjects, 0);
            GameObject thirdGO = Instantiate(third, tPos, Quaternion.Euler(new Vector3(0, 0, 0)));
            Animator thirdAnimator = thirdGO.GetComponentInChildren<Animator>();
            UpdatePlaceObjectScriptParams(thirdGO, false, true, false, true, 0);
            thirdAnimator.runtimeAnimatorController = (animatorIndex == 0) ? l2rAnimationController : snappingAnimationController;
        }

        if(count == 4)
        {
            Vector3 foPos = new Vector3((screenCenterXPoint / 2f) + 0.5f, spawningYCoordinate + 3 * yDistanceBetweenObjects, 0);
            GameObject fourthGO = Instantiate(fourth, foPos, Quaternion.Euler(new Vector3(0, 180, 360)));
            Animator fourthAnimator = fourthGO.GetComponentInChildren<Animator>();
            UpdatePlaceObjectScriptParams(fourthGO, false, true, false, true, 0);
            fourthAnimator.runtimeAnimatorController = (animatorIndex == 0) ? l2rAnimationController : snappingAnimationController;
        }
    }


    float PlaceMovingObjects(float yAxis, int count, string objectType)
    {
        float yDistanceBetweenObjects = (objectType == "blade") ? 2f : UnityEngine.Random.Range(0.75f, 1.25f);
        float xDistanceBetweenObjects = (screenWidth < 4.75f) ? 0.65f : UnityEngine.Random.Range(0.75f, 1.25f);
        GameObject first = objectsDict[objectType];
        GameObject second = objectsDict[objectType];
        GameObject third = objectsDict[objectType];
        GameObject fourth = objectsDict[objectType];
        
        // for integer minVal is inclusive and maxVal is exclusive
        int animatorIndex = UnityEngine.Random.Range(0, 2);
        float dynamicWidthForScaling = 4f;

        if (count == 1)
        {
            CreateL2RAnimation(animatorIndex, count, objectType, yDistanceBetweenObjects, yAxis);
            return yAxis;
        } else if (count == 2)
        {
            /*
             * movement = 0 for L2R movement
             * movement = 1,2 for aligned movement
             * movement = 3 for vertical movement
             */

            int movement = GetRandomWeightedIndex(new float[] { 0.5f, 0.2f, 0.2f, 0.1f });
            
            if (movement == 0)
            {
                CreateL2RAnimation(animatorIndex, count, objectType, yDistanceBetweenObjects, yAxis);
                return yAxis + yDistanceBetweenObjects;
            }  else if (movement == 1 || movement == 2)
            {
                int alignment = UnityEngine.Random.Range(15, 35);
                Vector3 fPos = new Vector3((screenCenterXPoint / 2f) - 0.5f, yAxis, 0);
                Vector3 sPos = new Vector3((screenCenterXPoint / 2f) + 0.5f, yAxis + yDistanceBetweenObjects, 0);

                GameObject firstGO, secondGO;
                if (movement == 1)
                {
                    firstGO = Instantiate(first, fPos, Quaternion.Euler(new Vector3(0, 0, (-1) * alignment)));
                    secondGO = Instantiate(second, sPos, Quaternion.Euler(new Vector3(0, 0, 180 - alignment)));
                } else
                {
                    firstGO = Instantiate(first, fPos, Quaternion.Euler(new Vector3(0, 0, alignment)));
                    secondGO = Instantiate(second, sPos, Quaternion.Euler(new Vector3(0, 0, (-1) * (180 - alignment))));
                }
                
                Animator firstAnimator = firstGO.GetComponentInChildren<Animator>();
                Animator secondAnimator = secondGO.GetComponentInChildren<Animator>();
                firstAnimator.runtimeAnimatorController = (animatorIndex == 0) ? l2rAnimationController : snappingAnimationController;
                secondAnimator.runtimeAnimatorController = (animatorIndex == 0) ? l2rAnimationController : snappingAnimationController;
                UpdatePlaceObjectScriptParams(firstGO, false, true, false, true, 0);
                UpdatePlaceObjectScriptParams(secondGO, false, true, false, true, 0);
            } else if(movement == 3)
            {
                Vector3 fPos = new Vector3((screenCenterXPoint / 2f) - xDistanceBetweenObjects, yAxis + dynamicWidthForScaling/2.8f, 0);
                Vector3 sPos = new Vector3((screenCenterXPoint / 2f) + xDistanceBetweenObjects, yAxis - dynamicWidthForScaling/2.8f, 0);
                GameObject firstGO = Instantiate(first, fPos, Quaternion.Euler(new Vector3(0, 0, -90)));
                GameObject secondGO = Instantiate(second, sPos, Quaternion.Euler(new Vector3(0, 0, 90)));
                UpdatePlaceObjectScriptParams(firstGO, true, false, false, true, dynamicWidthForScaling);
                UpdatePlaceObjectScriptParams(secondGO, true, false, false, true, dynamicWidthForScaling);
                Animator firstAnimator = firstGO.GetComponentInChildren<Animator>();
                Animator secondAnimator = secondGO.GetComponentInChildren<Animator>();
                firstAnimator.runtimeAnimatorController = (animatorIndex == 0) ? l2rAnimationController : snappingAnimationController;
                secondAnimator.runtimeAnimatorController = (animatorIndex == 0) ? l2rAnimationController : snappingAnimationController;
            }
            return yAxis + dynamicWidthForScaling;
        } else if(count == 3)
        {

            int movement = GetRandomWeightedIndex(new float[] { 0.7f, 0.3f });
            if (movement == 0)
            {
                CreateL2RAnimation(animatorIndex, count, objectType, yDistanceBetweenObjects, yAxis);
                return yAxis + (2f * yDistanceBetweenObjects);
            } else if(movement == 1)
            {
                Vector3 fPos = new Vector3((screenCenterXPoint / 2f), yAxis + dynamicWidthForScaling / 2.8f, 0);
                Vector3 sPos = new Vector3((screenCenterXPoint / 2f) - xDistanceBetweenObjects, yAxis - dynamicWidthForScaling / 2.8f, 0);
                Vector3 tPos = new Vector3((screenCenterXPoint / 2f) + xDistanceBetweenObjects, yAxis - dynamicWidthForScaling / 2.8f, 0);
                GameObject firstGO = Instantiate(first, fPos, Quaternion.Euler(new Vector3(0, 0, -90)));
                GameObject secondGO = Instantiate(second, sPos, Quaternion.Euler(new Vector3(0, 0, 90)));
                GameObject thirdGO = Instantiate(third, tPos, Quaternion.Euler(new Vector3(0, 0, 90)));
                UpdatePlaceObjectScriptParams(firstGO, true, false, false, true, dynamicWidthForScaling);
                UpdatePlaceObjectScriptParams(secondGO, true, false, false, true, dynamicWidthForScaling);
                UpdatePlaceObjectScriptParams(thirdGO, true, false, false, true, dynamicWidthForScaling);
                Animator firstAnimator = firstGO.GetComponentInChildren<Animator>();
                Animator secondAnimator = secondGO.GetComponentInChildren<Animator>();
                Animator thirdAnimator = thirdGO.GetComponentInChildren<Animator>();
                firstAnimator.runtimeAnimatorController = (animatorIndex == 0) ? l2rAnimationController : snappingAnimationController;
                secondAnimator.runtimeAnimatorController = (animatorIndex == 0) ? l2rAnimationController : snappingAnimationController;
                thirdAnimator.runtimeAnimatorController = (animatorIndex == 0) ? l2rAnimationController : snappingAnimationController;
            }
            return yAxis + dynamicWidthForScaling;
        } else
        {
            int movement = GetRandomWeightedIndex(new float[] { 0.7f, 0.3f });
            if (movement == 0)
            {
                CreateL2RAnimation(animatorIndex, count, objectType, yDistanceBetweenObjects, yAxis);
                return (yAxis + 3*yDistanceBetweenObjects);
            } else if(movement == 1)
            {
                Vector3 fPos = new Vector3((screenCenterXPoint / 2f) - (1.5f) * xDistanceBetweenObjects, yAxis + dynamicWidthForScaling / 2.8f, 0);
                Vector3 sPos = new Vector3((screenCenterXPoint / 2f) - (0.5f) * xDistanceBetweenObjects, yAxis - dynamicWidthForScaling / 2.8f, 0);
                Vector3 tPos = new Vector3((screenCenterXPoint / 2f) + (0.5f) * xDistanceBetweenObjects, yAxis + dynamicWidthForScaling / 2.8f, 0);
                Vector3 foPos = new Vector3((screenCenterXPoint / 2f) + (1.5f) * xDistanceBetweenObjects, yAxis - dynamicWidthForScaling / 2.8f, 0);
                GameObject firstGO = Instantiate(first, fPos, Quaternion.Euler(new Vector3(0, 0, -90)));
                GameObject secondGO = Instantiate(second, sPos, Quaternion.Euler(new Vector3(0, 0, 90)));
                GameObject thirdGO = Instantiate(third, tPos, Quaternion.Euler(new Vector3(0, 0, -90)));
                GameObject fourthGO = Instantiate(fourth, foPos, Quaternion.Euler(new Vector3(0, 0, 90)));
                UpdatePlaceObjectScriptParams(firstGO, true, false, false, true, dynamicWidthForScaling);
                UpdatePlaceObjectScriptParams(secondGO, true, false, false, true, dynamicWidthForScaling);
                UpdatePlaceObjectScriptParams(thirdGO, true, false, false, true, dynamicWidthForScaling);
                UpdatePlaceObjectScriptParams(fourthGO, true, false, false, true, dynamicWidthForScaling);
                Animator firstAnimator = firstGO.GetComponentInChildren<Animator>();
                Animator secondAnimator = secondGO.GetComponentInChildren<Animator>();
                Animator thirdAnimator = thirdGO.GetComponentInChildren<Animator>();
                Animator fourthAnimator = fourthGO.GetComponentInChildren<Animator>();
                firstAnimator.runtimeAnimatorController = (animatorIndex == 0) ? l2rAnimationController : snappingAnimationController;
                secondAnimator.runtimeAnimatorController = (animatorIndex == 0) ? l2rAnimationController : snappingAnimationController;
                thirdAnimator.runtimeAnimatorController = (animatorIndex == 0) ? l2rAnimationController : snappingAnimationController;
                fourthAnimator.runtimeAnimatorController = (animatorIndex == 0) ? l2rAnimationController : snappingAnimationController;
            }
            return yAxis + dynamicWidthForScaling;
        }
    }

    /*
     * xPosition = center means object shall be placed in center
     * xPosition = left means object shall be placed in right
     * xPosition = right means object shall be placed in left
     * xPosition = 2.54 means object shall be placed at position 2.54
     */
    void PlaceRotatingObjects(float yAxis, string objectType, string xPosition)
    {
        GameObject gameObject = objectsDict[objectType];
        
        float scaleFactor = (screenWidth < 4.75f) ? 0.9f : UnityEngine.Random.Range(1, 1.25f);

        if (xPosition == "center")
        {
            Vector3 objPos = new Vector3(0, yAxis, 0);
            gameObject = Instantiate(gameObject, objPos, transform.rotation);
            gameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            UpdatePlaceObjectScriptParams(gameObject, true, false, false, false, 0);
        } else if(xPosition == "left")
        {
            Vector3 objPos = new Vector3(-1, yAxis, 0);
            gameObject = Instantiate(gameObject, objPos, transform.rotation);
            gameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            UpdatePlaceObjectScriptParams(gameObject, true, true, false, false, 0);
        } else if(xPosition == "right") {
            Vector3 objPos = new Vector3(1, yAxis, 0);
            gameObject = Instantiate(gameObject, objPos, transform.rotation);
            gameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            UpdatePlaceObjectScriptParams(gameObject, true, true, false, false, 0);
        } else
        {
            Vector3 objPos = new Vector3(float.Parse(xPosition), yAxis, 0);
            gameObject = Instantiate(gameObject, objPos, transform.rotation);
            gameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            UpdatePlaceObjectScriptParams(gameObject, true, false, false, false, 0);
        }

        gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>().runtimeAnimatorController = rotateAnimationController;
    }

    float PlaceLauncher(float yAxis, int count)
    {
        GameObject launcher = objectsDict["launcher"];
        float xCoordinate = (UnityEngine.Random.Range(0, 2) == 0) ? (screenCenterXPoint / 2f) - 0.5f : (screenCenterXPoint / 2f) + 0.5f;
        Vector3 launcherPos = new Vector3(xCoordinate, yAxis, 0);
        GameObject launcherObj = Instantiate(launcher, launcherPos, transform.rotation);
        UpdatePlaceObjectScriptParams(launcherObj, false, true, false, false, 0);
        EnemyLauncher enemyLauncher = launcherObj.GetComponentInChildren<EnemyLauncher>();
        enemyLauncher.aimAtPlayer = true;
        enemyLauncher.playerPrefab = playerGO;

        if (count == 1)
            return yAxis;

        if(count == 2 || count == 3)
        {
            float newXCoordinate;
            if (xCoordinate < screenCenterXPoint/2f )
            {
                newXCoordinate = (screenCenterXPoint / 2f) + 0.5f;
            } else
            {
                newXCoordinate = (screenCenterXPoint / 2f) - 0.5f;
            }

            launcherPos.x = newXCoordinate;
            launcherPos.y = yAxis + 1f;
            launcherObj = Instantiate(launcher, launcherPos, transform.rotation);
            UpdatePlaceObjectScriptParams(launcherObj, false, true, false, false, 0);
            enemyLauncher = launcherObj.GetComponentInChildren<EnemyLauncher>();
            enemyLauncher.aimAtPlayer = true;
            enemyLauncher.playerPrefab = playerGO;

        }

        if (count == 2)
            return yAxis + 1f;

        if(count == 3)
        {
            launcherPos.x = xCoordinate;
            launcherPos.y = yAxis + 2f;
            launcherObj = Instantiate(launcher, launcherPos, transform.rotation);
            UpdatePlaceObjectScriptParams(launcherObj, false, true, false, false, 0);
            enemyLauncher = launcherObj.GetComponentInChildren<EnemyLauncher>();
            enemyLauncher.aimAtPlayer = true;
            enemyLauncher.playerPrefab = playerGO;
        }

        return yAxis + 2f;

    }

    float PlaceHorizontalPlatform(float yAxis, int count)
    {
        GameObject fHorizontalPlatform = objectsDict["hPlatform"];
        Vector3 platformPos = new Vector3(0, yAxis, 0);
        float alignment = (UnityEngine.Random.Range(-30, 30));
        Instantiate(fHorizontalPlatform, platformPos, Quaternion.Euler(new Vector3(0, 0, alignment)));

        if (count == 2)
            return yAxis + 5f;

        if (count == 3 || count ==4)
        {
            GameObject sHorizontalPlatform = objectsDict["hPlatform"];
            platformPos.y += 3.51f;
            sHorizontalPlatform = Instantiate(sHorizontalPlatform, platformPos, Quaternion.Euler(new Vector3(0, 0, alignment)));
            if(count == 3)
            {
                foreach(Transform child in sHorizontalPlatform.transform)
                {
                    if(child.gameObject.name == "LR Horizontal Platform (2)" || child.gameObject.name == "LR Horizontal Platform (3)")
                    {
                        child.gameObject.SetActive(false);
                    }
                }
                return yAxis + 7.5f;
            }
        }
        return yAxis + 10f;
    }

    void PlaceGrinder(float yAxis)
    {
        GameObject fHorizontalPlatform = objectsDict["grinder"];
        Vector3 platformPos = new Vector3(0, yAxis, 0);
        float alignment = (UnityEngine.Random.Range(-30, 30));
        Instantiate(fHorizontalPlatform, platformPos, Quaternion.Euler(new Vector3(0, 0, alignment)));
    }

    void PlaceStillCannon(float yAxis)
    {
        GameObject stillCannon = objectsDict["stillCannon"];

         /* 
          * 0 is for non blinking cannon shooting in regular intervals
          * 1 is for blinking cannon
         */
        int cannonType = UnityEngine.Random.Range(0, 2);
        
        /*
         * 0 is for cannon position in left
         * 1 is for cannon position in right
         */
        int leftOrRight = UnityEngine.Random.Range(0, 2);
        float xCoordinate = (leftOrRight == 0) ? -1.78f : 1.78f;
        Vector3 cannonPos = new Vector3(xCoordinate, yAxis, 0);

        float alignment = UnityEngine.Random.Range(-20, 20);

        if(leftOrRight == 1)
        {
            alignment += 180f;
        }

        stillCannon = Instantiate(stillCannon, cannonPos, Quaternion.Euler(new Vector3(0, 0, alignment)));
        Animator cannonAnimator = stillCannon.GetComponentInChildren<Animator>();
        EnemyLauncher enemyLauncher = stillCannon.GetComponent<EnemyLauncher>();
        if(cannonType == 0)
        {
            enemyLauncher.isBlinking = false;
            cannonAnimator.runtimeAnimatorController = stillCannonAnimationController;
        } else
        {
            enemyLauncher.isBlinking = true;
            enemyLauncher.playerPrefab = playerGO;
            cannonAnimator.runtimeAnimatorController = blinkingCannonAnimationController;
        }
    }


    void PlaceUpCannon(float yAxis)
    {
        GameObject upCannon = objectsDict["upCannon"];

        /*
         * 0 is for cannon position in left
         * 1 is for cannon position in right
         */
        int leftOrRight = UnityEngine.Random.Range(0, 2);

        float xCoordinate = (leftOrRight == 0) ? -1f : 1f;

        Vector3 cannonPos = new Vector3(xCoordinate, yAxis, 0);

        if (leftOrRight == 1)
            Instantiate(upCannon, cannonPos, Quaternion.Euler(new Vector3(180, 0, 180)));
        else
            Instantiate(upCannon, cannonPos, transform.rotation);
    }

    void PlaceBigSphere(float yAxis)
    {
        GameObject bigSphere = objectsDict["sphere"];
        GameObject sphereSupportingPlatform = objectsDict["sphereSupportingPlatform"];

        int leftOrRight = UnityEngine.Random.Range(0, 2);

        float xCoordinate = (leftOrRight == 0) ? -2.5f : 2.5f;

        Vector3 spherePos = new Vector3(xCoordinate, yAxis + 1.3f, 0);
        Vector3 platformPos = new Vector3(xCoordinate, yAxis, 0);

        float platformAlignment = (leftOrRight == 0) ? -21.5f : 21.5f;

        sphereSupportingPlatform = Instantiate(sphereSupportingPlatform, platformPos, Quaternion.Euler(new Vector3(0, 0, platformAlignment)));

        sphereSupportingPlatform.transform.localScale = new Vector3(1.5f, 1, 1);

        bigSphere = Instantiate(bigSphere, spherePos, transform.rotation);

        bigSphere.GetComponent<InitiateFall>().playerPrefab = playerGO;
    }

    void PlaceStillPlatform(float yAxis)
    {
        GameObject stillPlatform = objectsDict["stillPlatform"];

        int leftOrRight = UnityEngine.Random.Range(0, 2);

        float xCoordinate = (leftOrRight == 0) ? -2.5f : 2.5f;

        Vector3 stillPlatformPos = new Vector3(xCoordinate, yAxis, 0);

        Instantiate(stillPlatform, stillPlatformPos, transform.rotation);
    }

    float PlaceBigBox(float yAxis, int count)
    {
        string[] rotatingObjects = { "blade", "box", "square" };
        if (count == 2)
        {
            GameObject bigBoxWrapper = objectsDict["bigBox"];

            Vector3 wrapperPos = new Vector3(0, yAxis, 0);
            bigBoxWrapper = Instantiate(bigBoxWrapper, wrapperPos, transform.rotation);

            int boxPlacementCombinations = 3;
            GameObject fBox = bigBoxWrapper.transform.GetChild(0).gameObject;
            GameObject sBox = bigBoxWrapper.transform.GetChild(1).gameObject;

            if (boxPlacementCombinations == 0)
            {
                fBox.transform.localPosition = new Vector3(-4.2f, 2f, 0);
                sBox.transform.localPosition = new Vector3(4.2f, -2f, 0);

                bigBoxWrapper.transform.rotation = (UnityEngine.Random.Range(0,2) == 0) ? Quaternion.Euler(new Vector3(0, 0, 26f)) : Quaternion.Euler(new Vector3(0, 0, -26f));
            }
            else if (boxPlacementCombinations == 1)
            {
                bigBoxWrapper.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                fBox.transform.rotation = sBox.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45f));

                int objectRequired = (UnityEngine.Random.Range(0, 2));

                if(objectRequired == 0)
                {
                    fBox.transform.localPosition = new Vector3(-6.05f, 0, 0);
                    sBox.transform.localPosition = new Vector3(6.05f, 0, 0);
                } else
                {
                    fBox.transform.localPosition = new Vector3(-7.5f, 0, 0);
                    sBox.transform.localPosition = new Vector3(7.5f, 0, 0);
                    PlaceRotatingObjects(yAxis, "blade", "center");
                }
            }
            else if (boxPlacementCombinations == 2)
            {
                fBox.transform.localPosition = new Vector3(-5f, 0, 0);
                sBox.transform.localPosition = new Vector3(5f, 0, 0);

                var rend = fBox.transform.GetComponent<SpriteRenderer>();

                float minBound = rend.bounds.min.y;
                float maxBound = rend.bounds.max.y;

                PlaceRotatingObjects((minBound + 3*maxBound)/4f, rotatingObjects[UnityEngine.Random.Range(0,3)], "center");

                PlaceRotatingObjects((3*minBound + maxBound)/4f, rotatingObjects[UnityEngine.Random.Range(0, 3)], "center");
            }
            else if (boxPlacementCombinations == 3)
            {
                bigBoxWrapper.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                fBox.transform.localPosition = new Vector3(-7.3f, 0, 0);
                sBox.transform.localPosition = new Vector3(7.3f, 0, 0);

                var fRend = fBox.transform.GetComponent<SpriteRenderer>();
                var sRend = sBox.transform.GetComponent<SpriteRenderer>();

                Vector3 topRightArea = fRend.bounds.max;
                Vector3 bottomLeftArea = sRend.bounds.min;

                // Need to place moving object....


            }

            return yAxis + 5f;
        }
        else
        {
            GameObject bigBoxWrapperOne = objectsDict["bigBox"];
            GameObject bigBoxWrapperTwo = objectsDict["bigBox"];

            int objectRequired = (UnityEngine.Random.Range(0, 2));

            
            Vector3 wrapperPos = new Vector3(0, yAxis, 0);
            bigBoxWrapperOne = Instantiate(bigBoxWrapperOne, wrapperPos, transform.rotation);
            wrapperPos = new Vector3(0, yAxis + 5f, 0);
            bigBoxWrapperTwo = Instantiate(bigBoxWrapperTwo, wrapperPos, transform.rotation);

            bigBoxWrapperOne.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            bigBoxWrapperTwo.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

            GameObject fBox = bigBoxWrapperOne.transform.GetChild(0).gameObject;
            GameObject sBox = bigBoxWrapperOne.transform.GetChild(1).gameObject;

            fBox.transform.rotation = sBox.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45f));
            fBox.transform.localPosition = (objectRequired == 0) ? new Vector3(-6.05f, 0, 0) : new Vector3(-7.5f, 0, 0);
            sBox.transform.localPosition = (objectRequired == 0) ? new Vector3(6.05f, 0, 0) : new Vector3(7.5f, 0, 0);

            fBox = bigBoxWrapperTwo.transform.GetChild(0).gameObject;
            sBox = bigBoxWrapperTwo.transform.GetChild(1).gameObject;

            fBox.transform.rotation = sBox.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45f));
            fBox.transform.localPosition = (objectRequired == 0) ? new Vector3(-6.05f, 0, 0) : new Vector3(-7.5f, 0, 0);
            sBox.transform.localPosition = (objectRequired == 0) ? new Vector3(6.05f, 0, 0) : new Vector3(7.5f, 0, 0);

            if(objectRequired == 1)
            {
                PlaceRotatingObjects(yAxis, "blade", "center");
                PlaceRotatingObjects(yAxis + 5f, "blade", "center");
            }

            return yAxis + 10f;
        }
    }

    private float GenerateRotatingObjectCombinations(float yAxis, int numOfRotatingObjects, string objectType)
    {
        float margin = 2f;
        if (numOfRotatingObjects == 1)
            PlaceRotatingObjects(yAxis, objectType, "center");
        else if(numOfRotatingObjects == 2)
        {
            float xAxis = (screenWidth < 4.75f) ? UnityEngine.Random.Range(0.5f, 3f) : UnityEngine.Random.Range(1.3f, 3f);
            PlaceRotatingObjects(yAxis, objectType, xAxis.ToString());
            PlaceRotatingObjects(yAxis, objectType, (-1f * xAxis).ToString());
            return yAxis;
        } else if(numOfRotatingObjects == 3)
        {
            float xAxis = (screenWidth < 4.75f) ? UnityEngine.Random.Range(1f, 3f) : UnityEngine.Random.Range(1.3f, 3f);
            PlaceRotatingObjects(yAxis, objectType, "center");
            int placementCombination = UnityEngine.Random.Range(0, 3);
            if(placementCombination == 0)
            {
                PlaceRotatingObjects(yAxis + margin, objectType, xAxis.ToString());
                PlaceRotatingObjects(yAxis + margin, objectType, (-1f * xAxis).ToString());
            } else if(placementCombination == 1)
            {
                PlaceRotatingObjects(yAxis - margin, objectType, xAxis.ToString());
                PlaceRotatingObjects(yAxis - margin, objectType, (-1f * xAxis).ToString());
            } else if(placementCombination == 2)
            {
                PlaceRotatingObjects(yAxis, objectType, xAxis.ToString());
                PlaceRotatingObjects(yAxis, objectType, (-1f * xAxis).ToString());
                return yAxis;
            }
        } else if(numOfRotatingObjects == 4)
        {
            int placementCombination = UnityEngine.Random.Range(0, 2);

            if(placementCombination == 0)
            {
                float firstxAxis = (screenWidth < 4.75f) ? UnityEngine.Random.Range(0.5f, 1f) : UnityEngine.Random.Range(1.3f, 1.8f);
                float secondxAxis = (screenWidth < 4.75f) ? UnityEngine.Random.Range(1.2f, 3f) : UnityEngine.Random.Range(2f, 3f);

                int topOrDown = UnityEngine.Random.Range(0, 2);

                PlaceRotatingObjects(yAxis, objectType, firstxAxis.ToString());
                PlaceRotatingObjects(yAxis, objectType, (-1f * firstxAxis).ToString());

                if(topOrDown == 0)
                {
                    PlaceRotatingObjects(yAxis + margin, objectType, secondxAxis.ToString());
                    PlaceRotatingObjects(yAxis + margin, objectType, (-1f * secondxAxis).ToString());
                } else
                {
                    PlaceRotatingObjects(yAxis - margin, objectType, secondxAxis.ToString());
                    PlaceRotatingObjects(yAxis - margin, objectType, (-1f * secondxAxis).ToString());
                }
            } else if(placementCombination == 1)
            {
                float xAxis = (screenWidth < 4.75f) ? UnityEngine.Random.Range(1.3f, 3f) : UnityEngine.Random.Range(2f, 3f);
                PlaceRotatingObjects(yAxis, objectType, xAxis.ToString());
                PlaceRotatingObjects(yAxis, objectType, (-1f * xAxis).ToString());

                PlaceRotatingObjects(yAxis + margin, objectType, "center");
                PlaceRotatingObjects(yAxis - margin, objectType, "center");
            }

        } else if(numOfRotatingObjects == 5)
        {
            float xAxis = (screenWidth < 4.75f) ? UnityEngine.Random.Range(1.3f, 3f) : UnityEngine.Random.Range(2f, 3f);
            int placementCombination = UnityEngine.Random.Range(0, 2);
            if(placementCombination == 0)
            {
                PlaceRotatingObjects(yAxis, objectType, "center");
                PlaceRotatingObjects(yAxis, objectType, xAxis.ToString());
                PlaceRotatingObjects(yAxis, objectType, (-1f * xAxis).ToString());
                PlaceRotatingObjects(yAxis + margin, objectType, "center");
                PlaceRotatingObjects(yAxis - margin, objectType, "center");
            } else if(placementCombination == 1)
            {
                PlaceRotatingObjects(yAxis + margin, objectType, xAxis.ToString());
                PlaceRotatingObjects(yAxis + margin, objectType, (-1f * xAxis).ToString());
                PlaceRotatingObjects(yAxis, objectType, "center");
                PlaceRotatingObjects(yAxis - margin, objectType, xAxis.ToString());
                PlaceRotatingObjects(yAxis - margin, objectType, (-1f * xAxis).ToString());
            }
        }
        return yAxis + margin;
    }


    void Start()
    {
        var bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        var bottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
        screenWidth = bottomRight.x - bottomLeft.x;
        screenCenterXPoint = (bottomRight.x + bottomLeft.x) / 2f;
        CreateDictionary();

        int numOfObjects = UnityEngine.Random.Range(7, 8);

        float yAxis = transform.position.y + 10f;

        for (int i=0;i<numOfObjects;i++)
        {
            int objectIndex = UnityEngine.Random.Range(0, 12);
            string objectType = objectsDict.Keys.ElementAt(objectIndex);
            //string objectType = "stillPlatform";
            if (objectType == "square")
            {
                int count = GetRandomWeightedIndex(new float[] { 0.5f, 0.3f, 0.2f });
                yAxis = PlaceMovingObjects(yAxis, count + 2, objectType);
            } else if (objectType == "bigBox")
            {
                int numOfBoxes = UnityEngine.Random.Range(0, 2);
                yAxis = PlaceBigBox(yAxis, (numOfBoxes + 1) * 2);
            } else if (objectType == "sphere")
            {
                PlaceBigSphere(yAxis);
            } else if (objectType == "blade" || objectType == "box")
            {
                int rotateOrMove = (objectType == "blade") ? GetRandomWeightedIndex(new float[] { 0.7f, 0.3f }) : GetRandomWeightedIndex(new float[] { 0.4f, 0.6f });
                if (rotateOrMove == 0)
                {
                    int numOfRotatingObjects = UnityEngine.Random.Range(1, 6);
                    yAxis = GenerateRotatingObjectCombinations(yAxis, numOfRotatingObjects, objectType);
                } else
                {
                    int count = GetRandomWeightedIndex(new float[] { 0.5f, 0.3f, 0.2f });
                    yAxis = PlaceMovingObjects(yAxis, count + 2, objectType);
                }
            } else if(objectType == "launcher")
            {
                int numOfLaunchers = GetRandomWeightedIndex(new float[] { 0.1f, 0.55f, 0.35f });
                yAxis = PlaceLauncher(yAxis, numOfLaunchers + 1);
            } else if(objectType == "grinder")
            {
                PlaceGrinder(yAxis);
            } else if(objectType == "hPlatform")
            {
                int numOfPlatforms = GetRandomWeightedIndex(new float[] { 0.0f, 0.5f, 0.35f, 0.15f });
                yAxis = PlaceHorizontalPlatform(yAxis, numOfPlatforms + 1);
            } else if(objectType == "saw")
            {
                //PlaceRotatingObjects(yAxis, objectType, "center");
            } else if(objectType == "stillCannon")
            {
                PlaceStillCannon(yAxis);
            } else if(objectType == "upCannon")
            {
                PlaceUpCannon(yAxis);
            } else if(objectType == "stillPlatform")
            {
                PlaceStillPlatform(yAxis);
            }
            yAxis += 5f;
        }

        SetPlaySpaceAtRuntime(yAxis);

        hasLevelGenerationCompleted = true;

    }

    private void SetPlaySpaceAtRuntime(float yAxis)
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

            while(minBound < initialBound)
            {
                playSpaceGO.transform.position = new Vector3(playSpaceGO.transform.position.x, playSpaceGO.transform.position.y + 0.1f, playSpaceGO.transform.position.z);
                minBound = rend.bounds.min.y;
            }
            maxBound = rend.bounds.max.y;
            prevPlaySpaceScale = playSpaceGO.transform.localScale;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
