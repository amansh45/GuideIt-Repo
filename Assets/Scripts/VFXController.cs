using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class VFXController : MonoBehaviour
{
    [SerializeField] GameObject rippleEffectParticleSystem, explodeParticleSystem;
    [SerializeField] float rippleEffectDuration = 1f, explosionEffectDuration = 1.5f, cameraShakeDuration = 0.25f;
    [SerializeField] Camera mainCamera;
    ScreenRipple screenRippleClass;


    private void Start()
    {
        screenRippleClass = mainCamera.GetComponent<ScreenRipple>();
    }

    private void Update() {

    }

    public void InitiateRippleEffect(Vector3 position)
    {
        GameObject ripple = Instantiate(rippleEffectParticleSystem, position, transform.rotation);
        Destroy(ripple, rippleEffectDuration);
    }

    public void InitiateExplodeEffect(Vector3 position)
    {
        GameObject explosion = Instantiate(explodeParticleSystem, position, transform.rotation);
        Destroy(explosion, explosionEffectDuration);
    }

    public void InitiateCameraShakeEffect()
    {
        CameraManager camManager = FindObjectOfType<CameraManager>().GetComponent<CameraManager>();
        camManager.ShakeCamera(cameraShakeDuration);
    }

    public void InitiateScreenRippleEffect(Vector3 position)
    {
        screenRippleClass.ScreenRippleEffect(transform.position);
    }
}
