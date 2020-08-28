using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slowmotion : MonoBehaviour
{
    [SerializeField] float slowmoSpeed = 0.1f, normalSpeed = 1f;
    public List<GameObject> gameEntities = new List<GameObject>();
    List<Animator> animators = new List<Animator>();
    List<EnemyLauncher> granadeLaunchersInScene = new List<EnemyLauncher>();
    int numGameEntities, numAnimators = 0, numLaunchers = 0;
    float slowmoSpeedCache;

    void Start()
    {
        slowmoSpeedCache = slowmoSpeed;
        numGameEntities = gameEntities.Count;
        for (int i = 0; i < numGameEntities; i++)
        {
            var animator = gameEntities[i].GetComponent<Animator>();
            if (animator != null)
            {
                numAnimators += 1;
                animators.Add(animator);
            }

            var enemyLauncher = gameEntities[i].GetComponent<EnemyLauncher>();
            if (enemyLauncher != null)
            {
                numLaunchers += 1;
                granadeLaunchersInScene.Add(enemyLauncher);
            }
        }
    }

    public void customSlowmo(bool flag, float speed)
    {
        if(flag)
            slowmoSpeed = speed;
        else
            slowmoSpeed = slowmoSpeedCache;
        updateAnimations(flag);
    }

    public void updateAnimations(bool slowmo)
    {

        // update animations of all gameobjects.
        for (int i = 0; i < numAnimators; i++)
        {
            if (animators[i] != null)
            {
                if (slowmo)
                {
                    animators[i].speed = slowmoSpeed;
                }
                else
                {
                    animators[i].speed = normalSpeed;
                }
            }
        }

        // set enemy granade launchers to initialize new granades wrt slow-motion factor.
        for (int i = 0; i < numLaunchers; i++)
        {
            if (slowmo)
                granadeLaunchersInScene[i].SetSlowmoForGranadeLauncher(slowmoSpeed);
            else
                granadeLaunchersInScene[i].SetSlowmoForGranadeLauncher(1f);
        }


        // update moving speed for all active granades and scaling speed of all non active granades.
        Granade[] granades = FindObjectsOfType<Granade>();
        int activeGranades = granades.Length;
        for (int i = 0; i < activeGranades; i++)
        {
            Granade granadeClass = granades[i].GetComponent<Granade>();
            if (granadeClass.isGranadeActive())
            {
                if (slowmo)
                {
                    granadeClass.setGranadeSpeed(slowmoSpeed);
                }
                else
                {
                    granadeClass.setGranadeSpeed((1 / slowmoSpeed));
                }
            }
            else
            {
                if (slowmo)
                {
                    granadeClass.SetScaleFactor(slowmoSpeed);
                }
                else
                {
                    granadeClass.SetScaleFactor((1 / slowmoSpeed));
                }
            }
        }
    }

}
