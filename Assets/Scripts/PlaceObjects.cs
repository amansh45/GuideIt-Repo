using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObjects : MonoBehaviour
{
    public bool isRotating = false, placeWrtCornors = false, isCoinOrPlayer = false, scalingRequired = false;
    public float leftRightOffset = 0f, baseScreenWidth = 5.635593f, dynamicWidthForScaling = 0.0f;

    bool isFirstTimeLoad = true, objectInLeftHalf;
    float leftMost = int.MaxValue, rightMost = int.MinValue, coverage = int.MinValue;
    
    private void Start()
    {
        if (transform.position.x < (rightMost + leftMost) / 2f)
            objectInLeftHalf = true;
        else
            objectInLeftHalf = false;

        //float width = (float)(Camera.main.orthographicSize * 2.0 * Screen.width / Screen.height);

        var bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        var bottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
        float width = bottomRight.x - bottomLeft.x;
        

        if (scalingRequired)
        {
            if(dynamicWidthForScaling == 0.0f)
                gameObject.transform.localScale = new Vector3(width / baseScreenWidth, width / baseScreenWidth, width / baseScreenWidth);
            else
                gameObject.transform.localScale = new Vector3(dynamicWidthForScaling / baseScreenWidth, dynamicWidthForScaling / baseScreenWidth, dynamicWidthForScaling / baseScreenWidth);
        }
            

        if(!isCoinOrPlayer)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var rend = transform.GetChild(i).GetComponent<SpriteRenderer>();
                if (rend != null)
                {
                    leftMost = Mathf.Min(leftMost, rend.bounds.min.x);
                    rightMost = Mathf.Max(rightMost, rend.bounds.max.x);
                    coverage = Mathf.Max(coverage, (rend.bounds.max.x - rend.bounds.min.x));

                    if (isRotating)
                    {
                        coverage = Mathf.Max(coverage, (rend.bounds.max.y - rend.bounds.min.y));
                        float onRotateLeftmostReach = (transform.position.x - coverage / 2f);
                        float onRotateRightmostReach = (transform.position.x + coverage / 2f);
                        leftMost = Mathf.Min(leftMost, onRotateLeftmostReach);
                        rightMost = Mathf.Max(rightMost, onRotateRightmostReach);
                    }
                }
            }
        } else
        {
            var rend = GetComponent<SpriteRenderer>();
            if (rend != null)
            {
                leftMost = Mathf.Min(leftMost, rend.bounds.min.x);
                rightMost = Mathf.Max(rightMost, rend.bounds.max.x);
                coverage = Mathf.Max(coverage, (rend.bounds.max.x - rend.bounds.min.x));
            }
        }
    }

    void Update()
    {
        if (PersistentInformation.MarginsSet && isFirstTimeLoad)
        {
            Vector3 newPosition = transform.position;
            if(objectInLeftHalf)
            {
                if (leftMost < PersistentInformation.Left)
                    newPosition.x = transform.position.x + (PersistentInformation.Left - leftMost);
                else
                    newPosition.x = PersistentInformation.Left + coverage / 2f;

                newPosition.x += leftRightOffset;
            } else
            {
                if (rightMost > PersistentInformation.Right)
                    newPosition.x = transform.position.x - (rightMost - PersistentInformation.Right);
                else
                    newPosition.x = PersistentInformation.Right - coverage / 2f;

                newPosition.x -= leftRightOffset;
            }

            if(leftMost < PersistentInformation.Left || rightMost > PersistentInformation.Right || placeWrtCornors)
                transform.position = newPosition;
            

            isFirstTimeLoad = false;
        }
    }

    List<Vector3> rotatingPoints = new List<Vector3>();
    int numPoints = 0;
    float normalSpeed = 2f;
    int currentIndex = 0;
    bool isPlacementOnLineRequired = false;
    bool isLine = false;

    private void FixedUpdate()
    {
        if(isPlacementOnLineRequired)
        {
            int nextIndex = (currentIndex + 1) % numPoints;
            if (rotatingPoints[nextIndex] != transform.position)
            {
                Vector3 dir = rotatingPoints[nextIndex] - transform.position;
                float dist = dir.magnitude;
                dir = dir.normalized;
                float move = normalSpeed * Time.deltaTime;
                if (move > dist) 
                    move = dist;

                transform.Translate(dir * move);
            } else
            {
                currentIndex = nextIndex;
            }
        }
    }

    public void TriggerPlacementOnLine(List<Vector3> points, int index, bool isLine)
    {
        isPlacementOnLineRequired = true;
        rotatingPoints = points;
        currentIndex = index;
        this.isLine = isLine;
        numPoints = rotatingPoints.Count;
    }
}
