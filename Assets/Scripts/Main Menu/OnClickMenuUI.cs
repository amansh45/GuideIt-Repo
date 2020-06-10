using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickMenuUI : MonoBehaviour
{
    //[SerializeField] GameObject sceneLoader;
    //SceneLoader sceneLoaderClass;
    LevelController levelController = null;


    private void Start()
    {
        //sceneLoaderClass = sceneLoader.GetComponent<SceneLoader>();
    }

    public void LoadLevel()
    {
        //sceneLoaderClass.LoadSceneByName("Level1");
        SceneManager.LoadScene("Level1");
    }

    public void LoadMainMenu()
    {
        //sceneLoaderClass.LoadSceneByName("Main Menu");
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadSettings()
    {
        //sceneLoaderClass.LoadSceneByName("Settings");
        SceneManager.LoadScene("Settings");
    }

    public void OnMenuIconClick()
    {
        // Removing this null is giving error which is mapped to levelController from inspector.
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();
        levelController.clickedPauseButton();
    }

    public void OnResumeClick()
    {
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();
        levelController.clickedResumeButton();
    }

    public void OnRetryClick()
    {
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();
        levelController.clickedRetryButton();
    }

}
