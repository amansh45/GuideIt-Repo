using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    [SerializeField] float transitionTime = 0.5f;

    private void Start()
    {
        GameObject crossFadeCanvas = transform.GetChild(0).gameObject;
        transition = crossFadeCanvas.GetComponent<Animator>();
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadByName(sceneName));
    }

    public void LoadScene(int index)
    {
        StartCoroutine(LoadByIndex(index));
    }

    IEnumerator LoadByName(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadByIndex(int sceneIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneIndex);
    }
}
