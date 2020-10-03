using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeObjectPlacement : MonoBehaviour
{
    [System.Serializable]
    public struct Circle
    {
        public float xCoordinate;
        public float yCoordinate;
        public bool adjustWrtBorders;
        public float radius;
        public GameObject objectType;
        public int count;
    }

    [System.Serializable]
    public struct Line
    {
        public Vector3 firstPoint;
        public Vector3 secondPoint;

        // 0/1: This field tells, out of which points firstPoint or secondPoint the object should be spawned initially.
        public int objectSpawnPoint;
        public GameObject objectType;
        public bool placeWrtCorners;
    }

    [SerializeField] List<Circle> circlePlacement = new List<Circle>();
    [SerializeField] List<Line> linePlacement = new List<Line>();
    [SerializeField] RuntimeAnimatorController rotateAnimationController;
    [SerializeField] Sprite endPointsSprite;
    [SerializeField] GameObject slowMotionGO;
    [SerializeField] Camera mainCamera;
    [SerializeField] float cornerOffset = 0.5f;

    Slowmotion slowmotion;

    private void Start()
    {
        int numCircles = circlePlacement.Count;
        int numLines = linePlacement.Count;
        slowmotion = slowMotionGO.GetComponent<Slowmotion>();

        for(int i=0;i<numCircles;i++)
        {
            List<Vector3> points = Utils.DrawCircle(circlePlacement[i].xCoordinate, circlePlacement[i].yCoordinate, circlePlacement[i].adjustWrtBorders, circlePlacement[i].radius);
            int numPoints = points.Count;
            int assignedIndex = 0;
            int differenceBetweenIndex = numPoints / circlePlacement[i].count;
            for(int j=0;j<circlePlacement[i].count;j++)
            {
                GameObject objRevolving = Instantiate(circlePlacement[i].objectType, points[assignedIndex], transform.rotation);
                objRevolving.name = circlePlacement[i].objectType.name + " on circle";
                objRevolving.GetComponentInChildren<Animator>().runtimeAnimatorController = rotateAnimationController;
                PlaceObjects objectPlacementScript = objRevolving.GetComponent<PlaceObjects>();
                objectPlacementScript.isObjectAFreeBird = true;
                objectPlacementScript.scalingRequired = false;
                objectPlacementScript.TriggerPlacementOnLine(points, assignedIndex, false);
                assignedIndex = (assignedIndex + differenceBetweenIndex) % numPoints;
                slowmotion.gameEntities.Add(objRevolving);
            }
        }

        for(int i=0;i<numLines;i++)
        {
            Vector3 firstPoint = linePlacement[i].firstPoint, secondPoint = linePlacement[i].secondPoint;
            if(linePlacement[i].placeWrtCorners)
            {
                var bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
                var bottomRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0));
                firstPoint.x = bottomLeft.x + cornerOffset;
                secondPoint.x = bottomRight.x - cornerOffset;
            }

            Utils.DrawLine(firstPoint, secondPoint, endPointsSprite);
            GameObject objMoving = Instantiate(linePlacement[i].objectType, 
                (linePlacement[i].objectSpawnPoint == 0) ? firstPoint : secondPoint, 
                transform.rotation);
            objMoving.GetComponentInChildren<Animator>().runtimeAnimatorController = rotateAnimationController;
            objMoving.name = linePlacement[i].objectType.name + " on line";
            PlaceObjects objectPlacementScript = objMoving.GetComponent<PlaceObjects>();
            objectPlacementScript.isObjectAFreeBird = true;
            objectPlacementScript.scalingRequired = false;
            objectPlacementScript.leftRightOffset = 0f;
            objectPlacementScript.TriggerPlacementOnLine(new List<Vector3>() { firstPoint, secondPoint }, linePlacement[i].objectSpawnPoint, true);
            slowmotion.gameEntities.Add(objMoving);
        }
    }

}
