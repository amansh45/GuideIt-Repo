using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
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
    [SerializeField] RuntimeAnimatorController snappingAnimationController, l2rAnimationController, stillCannonAnimationController, blinkingCannonAnimationController;

    Dictionary<string, GameObject> objectsDict = new Dictionary<string, GameObject>();
    float screenWidth, screenCenterXPoint;

    void CreateDictionary()
    {
        objectsDict.Add("square", enemyObjects.square); //
        objectsDict.Add("bigBox", enemyObjects.bigBox); 
        objectsDict.Add("sphere", enemyObjects.sphere); //
        objectsDict.Add("blade", enemyObjects.blade); //
        objectsDict.Add("box", enemyObjects.box); // 
        objectsDict.Add("launcher", enemyObjects.launcher); //
        objectsDict.Add("grinder", enemyObjects.grinder); //
        objectsDict.Add("hPlatform", enemyObjects.hPlatform); //
        objectsDict.Add("saw", enemyObjects.saw); 
        objectsDict.Add("stillCannon", enemyObjects.stillCannon); //
        objectsDict.Add("upCannon", enemyObjects.upCannon); //
        objectsDict.Add("stillPlatform", enemyObjects.stillPlatform);
        objectsDict.Add("sphereSupportingPlatform", enemyObjects.sphereSupportingPlatform); //
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
        Vector3 sPos = new Vector3((screenCenterXPoint / 2f) + 0.5f, spawningYCoordinate + yDistanceBetweenObjects, 0);

        GameObject firstGO = Instantiate(first, fPos, Quaternion.Euler(new Vector3(0, 0, 0)));
        GameObject secondGO = Instantiate(second, sPos, Quaternion.Euler(new Vector3(0, 180, 360)));
        Animator firstAnimator = firstGO.GetComponentInChildren<Animator>();
        Animator secondAnimator = secondGO.GetComponentInChildren<Animator>();
        UpdatePlaceObjectScriptParams(firstGO, false, true, false, true, 0);
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


    void PlaceMovingObjects(float yAxis, int count, string objectType)
    {
        float yDistanceBetweenObjects = UnityEngine.Random.Range(0.75f, 1.25f);
        float xDistanceBetweenObjects = (screenWidth < 4.75f) ? 0.65f : UnityEngine.Random.Range(0.75f, 1.25f);
        GameObject first = objectsDict[objectType];
        GameObject second = objectsDict[objectType];
        GameObject third = objectsDict[objectType];
        GameObject fourth = objectsDict[objectType];
        
        // for integer minVal is inclusive and maxVal is exclusive
        int animatorIndex = UnityEngine.Random.Range(0, 2);

        if (count == 2)
        {
            /*
             * movement = 0 for L2R movement
             * movement = 1,2 for aligned movement
             * movement = 3 for vertical movement
             */

            int movement = UnityEngine.Random.Range(0, 4);
            if (movement == 0)
            {
                CreateL2RAnimation(animatorIndex, count, objectType, yDistanceBetweenObjects, yAxis);
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
                float dynamicWidthForScaling = 4f;
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
        } else if(count == 3)
        {

            int movement = UnityEngine.Random.Range(0, 2);
            if (movement == 0)
            {
                CreateL2RAnimation(animatorIndex, count, objectType, yDistanceBetweenObjects, yAxis);
            } else if(movement == 1)
            {
                float dynamicWidthForScaling = 4f;
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

        } else if(count == 4)
        {
            int movement = UnityEngine.Random.Range(0, 2);
            if (movement == 0)
            {
                CreateL2RAnimation(animatorIndex, count, objectType, yDistanceBetweenObjects, yAxis);
            } else if(movement == 1)
            {
                float dynamicWidthForScaling = 4f;
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
        }
    }

    void PlaceLauncher(float yAxis)
    {
        GameObject launcher = objectsDict["launcher"];
        float xCoordinate = (UnityEngine.Random.Range(0, 2) == 0) ? (screenCenterXPoint / 2f) - 0.5f : (screenCenterXPoint / 2f) + 0.5f;
        Vector3 launcherPos = new Vector3(xCoordinate, yAxis, 0);
        GameObject launcherObj = Instantiate(launcher, launcherPos, transform.rotation);
        UpdatePlaceObjectScriptParams(launcherObj, false, true, false, false, 0);
        EnemyLauncher enemyLauncher = launcherObj.GetComponentInChildren<EnemyLauncher>();
        enemyLauncher.aimAtPlayer = true;
        enemyLauncher.playerPrefab = playerGO;
    }

    void PlaceHorizontalPlatform(float yAxis, int count)
    {
        GameObject fHorizontalPlatform = objectsDict["hPlatform"];
        Vector3 platformPos = new Vector3(0, yAxis, 0);
        float alignment = (UnityEngine.Random.Range(-30, 30));
        Instantiate(fHorizontalPlatform, platformPos, Quaternion.Euler(new Vector3(0, 0, alignment)));

        if(count == 3 || count ==4)
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
            }
        }
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


    void Start()
    {
        var bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        var bottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
        screenWidth = bottomRight.x - bottomLeft.x;
        screenCenterXPoint = (bottomRight.x + bottomLeft.x) / 2f;
        CreateDictionary();
        Vector3 spawningPos = transform.position;

        /*
        for (int i=12;i>=0;i--)
        {
            spawningPos.y += 10f;
            GameObject currObject = Instantiate(objectsDict.Values.ElementAt(i), spawningPos, transform.rotation);
        }
        */


        PlaceStillPlatform(transform.position.y + 5f);
        PlaceStillPlatform(transform.position.y + 15f);
        PlaceStillPlatform(transform.position.y + 20f);
        PlaceStillPlatform(transform.position.y + 25f);


        /*

        PlaceBigSphere(transform.position.y + 5f);

        PlaceBigSphere(transform.position.y + 15f);
        PlaceBigSphere(transform.position.y + 20f);
        PlaceBigSphere(transform.position.y + 25f);
        PlaceBigSphere(transform.position.y + 30f);


        

        PlaceUpCannon(transform.position.y + 5f);
        PlaceUpCannon(transform.position.y + 15f);
        PlaceUpCannon(transform.position.y + 20f);
        PlaceUpCannon(transform.position.y + 25f);



        PlaceStillCannon(transform.position.y + 5f, 0);
        PlaceStillCannon(transform.position.y + 10f, 0);
        PlaceStillCannon(transform.position.y + 15f, 1);
        PlaceStillCannon(transform.position.y + 20f, 1);



        

        PlaceGrinder(transform.position.y + 5f);
        PlaceGrinder(transform.position.y + 15f);
        PlaceGrinder(transform.position.y + 20f);
        PlaceGrinder(transform.position.y + 25f);

        
        
        
        PlaceHorizontalPlatform(transform.position.y + 5f, 2);
        PlaceHorizontalPlatform(transform.position.y + 15f, 3);
        PlaceHorizontalPlatform(transform.position.y + 30f, 4);

        
        
        PlaceMovingObjects(transform.position.y + 3f, 2, 0, "square");
        PlaceMovingObjects(transform.position.y + 2 * 5f, 2, 1, "square");
        PlaceMovingObjects(transform.position.y + 3 * 5f, 2, 2, "square");
        PlaceMovingObjects(transform.position.y + 4 * 5f, 2, 3, "square");
        PlaceMovingObjects(transform.position.y + 5 * 5f, 3, 0, "square");
        PlaceMovingObjects(transform.position.y + 6 * 5f, 3, 1, "square");
        PlaceMovingObjects(transform.position.y + 7 * 5f, 4, 0, "square");
        PlaceMovingObjects(transform.position.y + 8 * 5f, 4, 1, "square");


        PlaceMovingObjects(transform.position.y + 9 * 5f, 2, 0, "box");
        PlaceMovingObjects(transform.position.y + 10 * 5f, 2, 1, "box");
        PlaceMovingObjects(transform.position.y + 11 * 5f, 2, 2, "box");
        PlaceMovingObjects(transform.position.y + 12 * 5f, 2, 3, "box");
        PlaceMovingObjects(transform.position.y + 13 * 5f, 3, 0, "box");
        PlaceMovingObjects(transform.position.y + 14 * 5f, 3, 1, "box");
        PlaceMovingObjects(transform.position.y + 15 * 5f, 4, 0, "box");
        PlaceMovingObjects(transform.position.y + 16 * 5f, 4, 1, "box");

        PlaceMovingObjects(transform.position.y + 17 * 5f, 2, 0, "blade");
        PlaceMovingObjects(transform.position.y + 18 * 5f, 2, 1, "blade");
        PlaceMovingObjects(transform.position.y + 19 * 5f, 2, 2, "blade");
        PlaceMovingObjects(transform.position.y + 20 * 5f, 2, 3, "blade");
        PlaceMovingObjects(transform.position.y + 21 * 5f, 3, 0, "blade");
        PlaceMovingObjects(transform.position.y + 22 * 5f, 3, 1, "blade");
        PlaceMovingObjects(transform.position.y + 23 * 5f, 4, 0, "blade");
        PlaceMovingObjects(transform.position.y + 24 * 5f, 4, 1, "blade");

        */

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
