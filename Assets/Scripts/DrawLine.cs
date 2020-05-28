using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] float thresholdDistanceBetweenCheckpoints = 0.3f;
    [SerializeField] GameObject slowmotion;

    GameObject currentLine;
    LineRenderer lineRenderer;
    //List<GameObject> fingerPositions;
    GameObject previousFingerPosition;

    Slowmotion slowmoClass;

    private void Start()
    {
        //fingerPositions = new List<GameObject>();
        slowmoClass = slowmotion.GetComponent<Slowmotion>();
    }

    private void Update()
    {

        Vector2 tempFingerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            slowmoClass.updateAnimations(true);
            CreateLine(tempFingerPos);
        }
        else if (Input.GetMouseButton(0))
        {
            try {
                if (Vector2.Distance(tempFingerPos, previousFingerPosition.transform.position) > thresholdDistanceBetweenCheckpoints)
                    UpdateLine(tempFingerPos);
            } catch(System.Exception exception) {
                CreateLine(tempFingerPos);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            slowmoClass.updateAnimations(false);
            playerPrefab.GetComponent<Player>().MoveBallNormally();
        }
    }

    void CreateLine(Vector2 initialFingerPos)
    {
        currentLine = Instantiate(linePrefab, initialFingerPos, Quaternion.identity) as GameObject;
        previousFingerPosition = currentLine;
        playerPrefab.GetComponent<Player>().SetWayPoints(previousFingerPosition, false);
    }


    void UpdateLine(Vector2 newFingerPos)
    {
        float angle = 0;
        /*
        if (numPoints > 1) {
            Vector2 secondVector = newFingerPos - fingerPositions[numPoints - 1];
            Vector2 firstVector = fingerPositions[numPoints - 1] - fingerPositions[numPoints - 2];
            angle = Vector2.Angle(firstVector, secondVector);
        }
        */
        var rotation = Quaternion.Euler(0, 0, angle);
        GameObject lineInstance = Instantiate(linePrefab, newFingerPos, rotation) as GameObject;
        previousFingerPosition = lineInstance;
        playerPrefab.GetComponent<Player>().SetWayPoints(previousFingerPosition, true);
    }

}