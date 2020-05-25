using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slowmotion : MonoBehaviour {

    [SerializeField] float slowmoSpeed = 0.1f, normalSpeed = 1f;
    [SerializeField] GameObject[] gameEntities;
    int numGameEntities;


    void Start() {
        numGameEntities = gameEntities.Length;
    }

    public void updateAnimations(bool slowmo) {

        // update animations of all gameobjects.
        for (int i = 0; i < numGameEntities; i++) {
            var animator = gameEntities[i].GetComponent<Animator>();
            if (animator != null) {
                if (slowmo) {
                    animator.speed = slowmoSpeed;
                } else {
                    animator.speed = normalSpeed;
                }
            }

            // set game launchers to initialize new granades wrt slow-motion factor.
            var granadeLauncher = gameEntities[i].GetComponent<GranadeLauncher>();
            if (granadeLauncher != null) {
                if (slowmo)
                    granadeLauncher.InitiateGranade(slowmoSpeed);
                else
                    granadeLauncher.InitiateGranade(1f);
            }

        }

        // update speed for all active granades.
        Granade[] granades = FindObjectsOfType<Granade>();
        int activeGranades = granades.Length;
        for (int i = 0; i < activeGranades; i++) {
            if (slowmo) {
                granades[i].GetComponent<Granade>().setGranadeSpeed(slowmoSpeed);
            } else {
                granades[i].GetComponent<Granade>().setGranadeSpeed((1 / slowmoSpeed));
            }
        }
    }
    
}
