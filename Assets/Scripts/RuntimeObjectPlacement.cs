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
    }

    [SerializeField] List<Circle> circlePlacement = new List<Circle>();
    [SerializeField] List<Line> linePlacement = new List<Line>();
    [SerializeField] RuntimeAnimatorController rotateAnimationController;
    [SerializeField] Sprite endPointsSprite;


    private void Start()
    {
        int numCircles = circlePlacement.Count;
        int numLines = linePlacement.Count;

        for(int i=0;i<numCircles;i++)
        {
            List<Vector3> points = Utils.DrawCircle(circlePlacement[i].xCoordinate, circlePlacement[i].yCoordinate, circlePlacement[i].adjustWrtBorders, circlePlacement[i].radius);
            int numPoints = points.Count;
            int assignedIndex = 0;
            int differenceBetweenIndex = numPoints / circlePlacement[i].count;
            for(int j=0;j<circlePlacement[i].count;j++)
            {
                GameObject gameObject = Instantiate(circlePlacement[i].objectType, points[assignedIndex], transform.rotation);
                gameObject.name = circlePlacement[i].objectType.name + " on circle";
                gameObject.GetComponentInChildren<Animator>().runtimeAnimatorController = rotateAnimationController;
                PlaceObjects objectPlacementScript = gameObject.GetComponent<PlaceObjects>();
                objectPlacementScript.TriggerPlacementOnLine(points, assignedIndex, false);
                assignedIndex = (assignedIndex + differenceBetweenIndex) % numPoints;
            }
        }

        for(int i=0;i<numLines;i++)
        {
            Utils.DrawLine(linePlacement[i].firstPoint, linePlacement[i].secondPoint, endPointsSprite);
            GameObject gameObject = Instantiate(linePlacement[i].objectType, 
                (linePlacement[i].objectSpawnPoint == 0) ? linePlacement[i].firstPoint : linePlacement[i].secondPoint, 
                transform.rotation);
            gameObject.name = linePlacement[i].objectType.name + " on line";
            PlaceObjects objectPlacementScript = gameObject.GetComponent<PlaceObjects>();
            objectPlacementScript.TriggerPlacementOnLine(new List<Vector3>() { linePlacement[i].firstPoint, linePlacement[i].secondPoint }, linePlacement[i].objectSpawnPoint, true);
        }
    }

}
