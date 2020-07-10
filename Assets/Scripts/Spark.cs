using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark : MonoBehaviour
{
    [SerializeField] Sprite upSprite, leftSprite, rightSprite;
    [SerializeField] float sparkSpeed = 400f, cornerOffset = 0.5f;

    Rigidbody2D sparkRigidbody;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        sparkRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = upSprite;
    }
    
    bool isLeftSpark, isActive = false;
    float destination;

    public void SetParams(bool IsLeftSpark, float Destination)
    {
        isLeftSpark = IsLeftSpark;
        destination = Destination;
        if (isLeftSpark)
            transform.position = new Vector3(transform.position.x + cornerOffset, transform.position.y, transform.position.z);
        else
            transform.position = new Vector3(transform.position.x - cornerOffset, transform.position.y, transform.position.z);
        isActive = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == gameObject.name)
            Destroy(gameObject);
    }

    void Update()
    {
        if(isActive)
        {
            float movementThisFrame = sparkSpeed * Time.deltaTime;
            if (transform.position.y <= destination)
                sparkRigidbody.velocity = new Vector2(0, movementThisFrame);
            else
            {
                if(isLeftSpark)
                {
                    spriteRenderer.sprite = rightSprite;
                    sparkRigidbody.velocity = new Vector2(movementThisFrame, 0);
                }
                else
                {
                    spriteRenderer.sprite = leftSprite;
                    sparkRigidbody.velocity = new Vector2((-1) * movementThisFrame, 0);
                }
                    
            }
        }

    }
}
