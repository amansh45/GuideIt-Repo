using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObjects : MonoBehaviour
{
    [SerializeField] bool isRotating = false, placeWrtCornors = false, isCoinOrPlayer = false, scalingRequired = false;
    [SerializeField] float leftRightOffset = 0.1f, baseScreenWidth = 5.61f;

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
            gameObject.transform.localScale = new Vector3(width / baseScreenWidth, width / baseScreenWidth, width / baseScreenWidth);

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
            
            Debug.Log("From " + gameObject.name + newPosition);

            if(leftMost < PersistentInformation.Left || rightMost > PersistentInformation.Right || placeWrtCornors)
                transform.position = newPosition;
            

            isFirstTimeLoad = false;
        }
    }
}
