using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Run,
    Still,
    Hover,
    Move,
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

    [SerializeField] float camShakeDuration = 1.5f;

    [SerializeField] GameObject vfxController, levelController;

    float currentXP = 5f;

    float ballSpeed;

    List<List<GameObject>> waypoints_buffer = new List<List<GameObject>>();
    List<GameObject> playerParticles = new List<GameObject>();
    List<Vector3> playerParticlesInitialScale = new List<Vector3>();

    int waypointIndex = 0, lineIndex = 0, numParticles;
    bool isMouseDown = false;
    bool reducePlayerScaleToZero = false;

    Rigidbody2D playerRigidBody;
    TaskHandler taskHandlerClass;
    VFXController vfxControllerClass;
    LevelController levelControllerClass;

    public PlayerState playerState;
    public float timeUntilPlayerShouldNotDie = 0f;
    List<string> nearMissObjectNames = new List<string>();
    PlayerStatistics playerStats;

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
        moveSpeed = 0.1f * runSpeed;
        playerRigidBody = GetComponent<Rigidbody2D>();
        taskHandlerClass = FindObjectOfType<TaskHandler>();
        playerState = PlayerState.Still;
        vfxControllerClass = FindObjectOfType<VFXController>();
        if(levelController != null)
            levelControllerClass = levelController.GetComponent<LevelController>();
        playerStats = FindObjectOfType<PlayerStatistics>();
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
        playerState = PlayerState.Move;
        if (!isLineCreated)
        {
            List<GameObject> waypoints = new List<GameObject>();
            waypoints.Add(point);
            waypoints_buffer.Add(waypoints);
        }
        else
        {
            waypoints_buffer[waypoints_buffer.Count-1].Add(point);
        }
    }

    bool ballCameToRest = false;

    void Update()
    {
        if(timeUntilPlayerShouldNotDie > 0)
        {
            timeUntilPlayerShouldNotDie -= Time.deltaTime;
        }

        if (playerState == PlayerState.Run)
            ballSpeed = runSpeed;
        else if (playerState == PlayerState.Still)
            ballSpeed = 0f;
        else if (playerState == PlayerState.Hover)
        {
            ballCameToRest = true;
            ballSpeed = hoverSpeed;
        }
        else if (playerState == PlayerState.Move)
            ballSpeed = moveSpeed;
        
        Move();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == ObjectsDescription.EnemyObject.ToString() || other.gameObject.tag == ObjectsDescription.EnemyLauncher.ToString())
            Die(other.gameObject);
        else if (other.gameObject.tag == ObjectsDescription.FinishLine.ToString())
            LevelComplete();
        else if (other.gameObject.name.Replace(" ", string.Empty) == ObjectsDescription.NearMissBladeGO.ToString())
        {
            string objectName = other.gameObject.transform.parent.parent.gameObject.name;

            if (nearMissObjectNames.Contains(objectName))
                return;

            nearMissObjectNames.Add(objectName);

            taskHandlerClass.UpdateLevelTaskState(ObjectsDescription.Blade, TaskTypes.NearMiss, TaskCategory.CountingTask, new List<string>() { });
        } else if(other.gameObject.name.Replace(" ", string.Empty) == ObjectsDescription.NearMissBoxGO.ToString())
        {
            string objectName = other.gameObject.transform.parent.parent.gameObject.name;
            
            if (nearMissObjectNames.Contains(objectName))
                return;

            nearMissObjectNames.Add(objectName);
            taskHandlerClass.UpdateLevelTaskState(ObjectsDescription.Box, TaskTypes.NearMiss, TaskCategory.CountingTask, new List<string>() { });
        } else if(other.gameObject.name.Replace(" ", string.Empty) == ObjectsDescription.NearMissSquareGO.ToString())
        {
            string objectName = other.gameObject.transform.parent.parent.gameObject.name;

            Debug.Log(objectName);

            if (nearMissObjectNames.Contains(objectName))
                return;

            nearMissObjectNames.Add(objectName);
            taskHandlerClass.UpdateLevelTaskState(ObjectsDescription.Square, TaskTypes.NearMiss, TaskCategory.CountingTask, new List<string>() { });
        }
    }

    void Die(GameObject collider)
    {
        /*
        if(timeUntilPlayerShouldNotDie <= 0)
        {
            Debug.Log("Player Died...");
            levelControllerClass.ShowRetryCanvas(camShakeDuration);

            playerState = PlayerState.Still;

            gameObject.SetActive(false);
            vfxControllerClass.PlayerDied(transform.position, collider, camShakeDuration);
        }
        */
    }

    void LevelComplete()
    {
        string levelName = gameObject.scene.name;
        string[] levelIdentity = levelName.Split('.');
        int currentChapterIndex = int.Parse(levelIdentity[0]);
        int currentLevelIndex = int.Parse(levelIdentity[1]);

        taskHandlerClass.UpdateLevelTaskState(ObjectsDescription.Player, TaskTypes.Hover, TaskCategory.ImmediateActionTask, new List<string>() { (!ballCameToRest).ToString() });
        taskHandlerClass.UpdateLevelTaskState(ObjectsDescription.Player, TaskTypes.NoHit, TaskCategory.ImmediateActionTask, new List<string>() { (playerStats.chaptersList[currentChapterIndex].LevelsInChapter[currentLevelIndex].numTimesLevelFailed == 0).ToString() });
        taskHandlerClass.UpdateLevelTaskState(ObjectsDescription.Player, TaskTypes.NoNearMiss, TaskCategory.ImmediateActionTask, new List<string>() {  });
        taskHandlerClass.FinalizeTasks();
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
            if (playerState != PlayerState.Still)
            {
                playerState = PlayerState.Hover;
                ballSpeed = hoverSpeed;
            }
            float movementThisFrame = ballSpeed * Time.deltaTime;
            playerRigidBody.velocity = new Vector2(0, movementThisFrame);
        }
    }

    public bool reachedXPToShoot()
    {
        return currentXP >= shootXPThreshold;
    }

}
