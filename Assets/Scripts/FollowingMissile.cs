using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingMissile : MonoBehaviour
{
    [SerializeField] float missileSpeed = 5f, rotateSpeed = 100f, thresholdDistance = 10f;
    [SerializeField] Transform player;
    [SerializeField] float cameraShakeDuration = 0.25f;
    [SerializeField] GameObject missileIndicator;
    private Rigidbody2D rigidbody;
    VFXController vfxControllerClass;
    bool missileMoving = false;
    GameObject indicatorInstance;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        vfxControllerClass = FindObjectOfType<VFXController>().GetComponent<VFXController>();
        spawnMissileIndicator();
    }

    private void spawnMissileIndicator()
    {
        indicatorInstance = Instantiate(missileIndicator, transform.position, transform.rotation);
        indicatorInstance.transform.parent = FindObjectOfType<Background>().transform;
        indicatorInstance.name = "Missile Indicator";
    }

    private void FixedUpdate()
    {
        if(Vector3.Distance(player.position, transform.position) <= thresholdDistance || missileMoving)
        {
            if (!missileMoving)
            {
                indicatorInstance.SetActive(true);
                indicatorInstance.GetComponent<MissileIndicator>().InitiateIndicator(gameObject, player.gameObject);
            }

            Vector2 direction = (Vector2)player.position - rigidbody.position;
            direction.Normalize();
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            rigidbody.angularVelocity = -rotateAmount * rotateSpeed;
            rigidbody.velocity = transform.up * missileSpeed;

            missileMoving = true;
            
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (tag == ObjectsDescription.EnemyProjectile.ToString())
        {
            if (other.gameObject.tag == ObjectsDescription.Player.ToString() || other.gameObject.tag == ObjectsDescription.PlayerProjectile.ToString())
            {
                Destroy(gameObject);
                vfxControllerClass.InitiateCameraShakeEffect(cameraShakeDuration);
                vfxControllerClass.InitiateScreenRippleEffect(transform.position);
                vfxControllerClass.InitiateExplodeEffect(transform.position);
            }

            else if (other.gameObject.tag == ObjectsDescription.EnemyObject.ToString())
            {
                Destroy(gameObject);
                vfxControllerClass.InitiateExplodeEffect(transform.position);
            }

        }
        
    }
}
