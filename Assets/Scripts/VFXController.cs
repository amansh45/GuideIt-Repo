using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    [SerializeField] GameObject rippleEffectParticleSystem;
    [SerializeField] GameObject explodeParticleSystem;

    public void InitiateRippleEffect(Vector3 position, float effectDuration) {
        GameObject ripple = Instantiate(rippleEffectParticleSystem, position, transform.rotation);
        Destroy(ripple, effectDuration);
    }
    
    public void InitiateExplodeEffect(Vector3 position, float effectDuration) {
        GameObject explosion = Instantiate(explodeParticleSystem, position, transform.rotation);
        Destroy(explosion, effectDuration);
    }
}
