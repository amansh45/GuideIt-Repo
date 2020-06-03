using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Run,
    Still,
    Hover,
}

public class Player : MonoBehaviour
{

    // Speed of the ball when mouseDown
    float moveSpeed;

    // Speed of the ball when mouseUp
    [SerializeField] float runSpeed = 10f;

    // Hovering speed of the ball when no path is present in the screen..
    [SerializeField] float hoverSpeed = 25f;

    [SerializeField] float shootXPThreshold = 5f;

    float currentXP = 5f;

    float ballSpeed;

    List<List<GameObject>> waypoints_buffer = new List<List<GameObject>>();
    List<GameObject> playerParticles = new List<GameObject>();
    List<Vector3> playerParticlesInitialScale = new List<Vector3>();

    int waypointIndex = 0, lineIndex = 0, numParticles;
    bool isMouseDown = false;
    bool reducePlayerScaleToZero = false;

    Rigidbody2D playerRigidBody;

    void Start()
    {
        foreach (Transform child in transform)
        {
            foreach (Transform particles in child)
            {
                playerParticles.Add(particles.gameObject);
                playerParticlesInitialScale.Add(particles.localScale);
            }
        }
        numParticles = playerParticles.Count;
        ballSpeed = hoverSpeed;
        moveSpeed = 0.1f * runSpeed;
        playerRigidBody = GetComponent<Rigidbody2D>();
    }

    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
        for (int i = 0; i < numParticles; i++)
        {
            if (scale == 0)
                playerParticles[i].transform.localScale = new Vector3(scale, scale, scale);
            else
                playerParticles[i].transform.localScale = playerParticlesInitialScale[i];
        }
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
    }

    public void MovePlayer(PlayerState speed)
    {
        if (speed == PlayerState.Run)
            ballSpeed = runSpeed;
        else if (speed == PlayerState.Still)
            ballSpeed = 0f;
        else if (speed == PlayerState.Hover)
            ballSpeed = hoverSpeed;
    }


    void Update()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == ObjectsDescription.EnemyObject.ToString())
            Die();
        else if (other.gameObject.tag == ObjectsDescription.FinishLine.ToString())
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
                    //targetPosition.SetActive(false);
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
            float movementThisFrame = ballSpeed * Time.deltaTime;
            playerRigidBody.velocity = new Vector2(0, movementThisFrame);
        }
    }

    public bool reachedXPToShoot()
    {
        return currentXP >= shootXPThreshold;
    }

}
