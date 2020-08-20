using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiateFall : MonoBehaviour
{
    public GameObject playerPrefab;
    [SerializeField] float thresholdDistance = 10f;
    bool isFalling = false;

    void Start()
    {

    }

    void Update()
    {
        if (playerPrefab != null && Vector2.Distance(transform.position, playerPrefab.transform.position) <= thresholdDistance && !isFalling)
        {
            Debug.Log(Vector2.Distance(transform.position, playerPrefab.transform.position));
            gameObject.AddComponent<Rigidbody2D>();
            isFalling = true;
        }
    }
}
