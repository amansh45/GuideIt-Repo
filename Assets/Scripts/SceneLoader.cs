using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    Animator transition;
    [SerializeField] float transitionTime = 1f;

    private void Start()
    {
        transition = GetComponent<Animator>();
    }

    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadByName(sceneName));
    }

    public void LoadSceneByIndex(int index)
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
