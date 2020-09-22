using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileIndicator : MonoBehaviour
{
    GameObject missileGO, playerGO, playerMovementArea;
    bool indicatorInitiated = false;

    struct Line
    {
        public Vector2 firstPoint;
        public Vector2 secondPoint;

        public Line(Vector2 fPoint, Vector2 sPoint)
        {
            firstPoint = fPoint;
            secondPoint = sPoint;
        }
    }

    Vector2? LSegsIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
    {
        // Get A,B of first line - points : ps1 to pe1
        float A1 = pe1.y - ps1.y;
        float B1 = ps1.x - pe1.x;
        // Get A,B of second line - points : ps2 to pe2
        float A2 = pe2.y - ps2.y;
        float B2 = ps2.x - pe2.x;

        // Get delta and check if the lines are parallel
        float delta = A1 * B2 - A2 * B1;
        if (delta == 0) return null;

        // Get C of first and second lines
        float C2 = A2 * ps2.x + B2 * ps2.y;
        float C1 = A1 * ps1.x + B1 * ps1.y;
        //invert delta to make division cheaper
        float invdelta = 1 / delta;
        // now return the Vector2 intersection point
        return new Vector2((B2 * C1 - B1 * C2) * invdelta, (A1 * C2 - A2 * C1) * invdelta);
    }

    Vector2? LSegRec_IntersPoint_v02(Vector2 p1, Vector2 p2, float min_x, float min_y, float max_x, float max_y)
    {
        Vector2? intersection;

        if (p2.x < min_x) //If the second point of the segment is at left/bottom-left/top-left of the AABB
        {
            if (p2.y > min_y && p2.y < max_y) { return LSegsIntersectionPoint(p1, p2, new Vector2(min_x, min_y), new Vector2(min_x, max_y)); } //If it is at the left
            else if (p2.y < min_y) //If it is at the bottom-left
            {
                intersection = LSegsIntersectionPoint(p1, p2, new Vector2(min_x, min_y), new Vector2(max_x, min_y));
                if (intersection == null) intersection = LSegsIntersectionPoint(p1, p2, new Vector2(min_x, min_y), new Vector2(min_x, max_y));
                return intersection;
            }
            else //if p2.y > max_y, i.e. if it is at the top-left
            {
                intersection = LSegsIntersectionPoint(p1, p2, new Vector2(min_x, max_y), new Vector2(max_x, max_y));
                if (intersection == null) intersection = LSegsIntersectionPoint(p1, p2, new Vector2(min_x, min_y), new Vector2(min_x, max_y));
                return intersection;
            }
        }

        else if (p2.x > max_x) //If the second point of the segment is at right/bottom-right/top-right of the AABB
        {
            if (p2.y > min_y && p2.y < max_y) { return LSegsIntersectionPoint(p1, p2, new Vector2(max_x, min_y), new Vector2(max_x, max_y)); } //If it is at the right
            else if (p2.y < min_y) //If it is at the bottom-right
            {
                intersection = LSegsIntersectionPoint(p1, p2, new Vector2(min_x, min_y), new Vector2(max_x, min_y));
                if (intersection == null) intersection = LSegsIntersectionPoint(p1, p2, new Vector2(max_x, min_y), new Vector2(max_x, max_y));
                return intersection;
            }
            else //if p2.y > max_y, i.e. if it is at the top-left
            {
                intersection = LSegsIntersectionPoint(p1, p2, new Vector2(min_x, max_y), new Vector2(max_x, max_y));
                if (intersection == null) intersection = LSegsIntersectionPoint(p1, p2, new Vector2(max_x, min_y), new Vector2(max_x, max_y));
                return intersection;
            }
        }

        else //If the second point of the segment is at top/bottom of the AABB
        {
            if (p2.y < min_y) return LSegsIntersectionPoint(p1, p2, new Vector2(min_x, min_y), new Vector2(max_x, min_y)); //If it is at the bottom
            if (p2.y > max_y) return LSegsIntersectionPoint(p1, p2, new Vector2(min_x, max_y), new Vector2(max_x, max_y)); //If it is at the top
        }

        return null;

    }


    public void InitiateIndicator(GameObject missile, GameObject player)
    {
        missileGO = missile;
        playerGO = player;
        playerMovementArea = player.GetComponent<PlayerActions>().playerMovementArea;
        indicatorInitiated = true;
    }


    void Update()
    {
        if(indicatorInitiated)
        {

            Vector3 movementAreaCoordinates = playerMovementArea.transform.position;
   
            Vector3 missilePos = missileGO.transform.position;
            
            float playerMovementAreaLeftX = movementAreaCoordinates.x - (playerMovementArea.transform.localScale.x / 2f), 
                playerMovementAreaRightX = movementAreaCoordinates.x + (playerMovementArea.transform.localScale.x / 2f),
                playerMovementAreaTopY = movementAreaCoordinates.y + (playerMovementArea.transform.localScale.y / 2f),
                playerMovementAreaBottomY = movementAreaCoordinates.y - (playerMovementArea.transform.localScale.y / 2f) + 0.06f;


            if (missilePos.x > playerMovementAreaRightX || missilePos.x < playerMovementAreaLeftX || missilePos.y < playerMovementAreaBottomY || missilePos.y > playerMovementAreaTopY)
            {
                GetComponent<SpriteRenderer>().enabled = true;
                Vector2? positionOfIndicator = LSegRec_IntersPoint_v02((Vector2)playerGO.transform.position, (Vector2)missilePos,
                    playerMovementAreaLeftX, playerMovementAreaBottomY, playerMovementAreaRightX, playerMovementAreaTopY);
                if (positionOfIndicator != null)
                {
                    Vector2 finalPos = (Vector2)positionOfIndicator;
                    gameObject.transform.position = new Vector3(finalPos.x, finalPos.y, -2f);
                } 
            } else
            {
                GetComponent<SpriteRenderer>().enabled = false;
            }
            
        }

    }
}
