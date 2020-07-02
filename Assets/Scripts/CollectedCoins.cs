using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectedCoins : MonoBehaviour
{
    [SerializeField] float coinMovementSpeed = 7f;

    public Vector3 destinationPos;
    
    void Update()
    {
        if(destinationPos != null)
        {
            var movementThisFrame = coinMovementSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, destinationPos, movementThisFrame);
        }
        if (transform.position == destinationPos)
            Destroy(gameObject);
    }
}
