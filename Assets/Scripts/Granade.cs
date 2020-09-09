using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{

    [SerializeField] float granadeSpeed = 200f, shrinkFactorOnLaunch = 0.3f, cameraShakeDuration = 0.25f;
    VFXController vfxControllerClass;

    bool isGranadeMoving = false;
    float startScale = 0f, scaleFactor = 0.01f;
    Rigidbody2D granadeRigidBody;
    TaskHandler taskHandlerClass;

    private void Start()
    {
        transform.localScale = new Vector3(startScale, startScale, startScale);
        vfxControllerClass = FindObjectOfType<VFXController>().GetComponent<VFXController>();
        taskHandlerClass = FindObjectOfType<TaskHandler>();
        granadeRigidBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if(isGranadeMoving)
        {
            float projectileSpeed = granadeSpeed * Time.deltaTime;
            granadeRigidBody.velocity = transform.right * projectileSpeed;
        }
    }

    void Update()
    {
        if (!isGranadeMoving)
        {
            if (startScale < 1f)
            {
                startScale += scaleFactor;
                transform.localScale = new Vector3(startScale, startScale, startScale);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (tag == ObjectsDescription.EnemyProjectile.ToString() && isGranadeMoving)
        {
            if(other.gameObject.tag == ObjectsDescription.PlayerProjectile.ToString())
            {
                taskHandlerClass.UpdateLevelTaskState(ObjectsDescription.EnemyProjectile, TaskTypes.Destroy, TaskCategory.CountingTask, new List<string>() { });
            }

            if (other.gameObject.tag == ObjectsDescription.Player.ToString() || other.gameObject.tag == ObjectsDescription.PlayerProjectile.ToString())
            {
                Destroy(gameObject);
                vfxControllerClass.InitiateCameraShakeEffect(cameraShakeDuration);
                vfxControllerClass.InitiateScreenRippleEffect(transform.position);
                vfxControllerClass.InitiateExplodeEffect(transform.position);
            }
            
            else if(other.gameObject.tag == ObjectsDescription.EnemyObject.ToString())
            {
                Destroy(gameObject);
                vfxControllerClass.InitiateExplodeEffect(transform.position);
            }
            
        }
        else if (tag == ObjectsDescription.PlayerProjectile.ToString())
        {
            if (other.gameObject.tag == ObjectsDescription.EnemyLauncher.ToString())
            {
                vfxControllerClass.InitiateCameraShakeEffect(cameraShakeDuration);
                GameObject launcher = other.transform.parent.gameObject;
                vfxControllerClass.InitiateScreenRippleEffect(launcher.transform.position);
                vfxControllerClass.InitiateExplodeEffect(launcher.transform.position);
                taskHandlerClass.UpdateLevelTaskState(ObjectsDescription.EnemyLauncher, TaskTypes.Destroy, TaskCategory.CountingTask, new List<string>() { });
                Destroy(launcher);
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name.Replace(" ",string.Empty).Contains(ObjectsDescription.BigFallingSphere.ToString()))
        {
            taskHandlerClass.UpdateLevelTaskState(ObjectsDescription.BigFallingSphere, TaskTypes.Collide, TaskCategory.CountingTask, new List<string>() { });
            Destroy(gameObject);
            vfxControllerClass.InitiateCameraShakeEffect(cameraShakeDuration);
            vfxControllerClass.InitiateScreenRippleEffect(transform.position);
            vfxControllerClass.InitiateExplodeEffect(transform.position);
        }
            
    }

    public void SetScaleFactor(float slomotionScaleFactor)
    {
        scaleFactor *= slomotionScaleFactor;
    }

    public void MoveGranade()
    {
        isGranadeMoving = true;
        transform.localScale = new Vector3(1f, (1f - shrinkFactorOnLaunch), 1f);
    }

    public void setGranadeSpeed(float slowmotionFactor)
    {
        granadeSpeed *= slowmotionFactor;
    }

    public bool isGranadeActive()
    {
        return isGranadeMoving;
    }

}
