using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    // Speed of the ball when mouseDown
    [SerializeField] float moveSpeed = 1.2f;

    // Speed of the ball when mouseUp
    [SerializeField] float runSpeed = 10f;

    // Hovering speed of the ball when no path is present in the screen..
    [SerializeField] float idleSpeed = 25f;

    float ballSpeed;

    List<List<GameObject>> waypoints_buffer = new List<List<GameObject>>();

    int waypointIndex = 0, lineIndex = 0;
    bool isMouseDown = false;

    void Start()
    {
        ballSpeed = idleSpeed;
    }

    public void SetWayPoints(GameObject point, bool isLineCreated)
    {
        ballSpeed = moveSpeed;
        if (!isLineCreated)
        {
            List<GameObject> waypoints = new List<GameObject>();
            waypoints.Add(point);
            waypoints_buffer.Add(waypoints);
        }
        else
        {
            waypoints_buffer[lineIndex].Add(point);
        }
        Debug.Log("Ball speed is:" + ballSpeed);
    }

    public void MoveBallNormally()
    {
        ballSpeed = runSpeed;
    }


    void Update()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Enemy>() != null)
            Die();
        else if (other.gameObject.GetComponent<FinishLine>() != null)
            LevelComplete();
    }

    void Die()
    {
        Debug.Log("Player Died...");
    }

    void LevelComplete()
    {
        Debug.Log("Level Complete...");
    }


    private void Move()
    {
        // check if a path is present to move the player on it
        if (lineIndex <= waypoints_buffer.Count - 1)
        {
            if (waypointIndex <= waypoints_buffer[lineIndex].Count - 1)
            {
                var targetPosition = waypoints_buffer[lineIndex][waypointIndex];
                var movementThisFrame = ballSpeed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, targetPosition.transform.position, movementThisFrame);
                if (transform.position.x == targetPosition.transform.position.x && transform.position.y == targetPosition.transform.position.y)
                {
                    waypointIndex += 1;
                    Destroy(targetPosition);
                }
            }
            else if (waypointIndex == waypoints_buffer[lineIndex].Count && waypointIndex != 0)
            {
                lineIndex += 1;
                waypointIndex = 0;
            }
        }
        else
        {
            float movementThisFrame = idleSpeed * Time.deltaTime;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, movementThisFrame);
        }
    }

}
