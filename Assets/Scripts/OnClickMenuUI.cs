using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class OnClickMenuUI : MonoBehaviour
{
    //[SerializeField] GameObject sceneLoader;
    //SceneLoader sceneLoaderClass;
    LevelController levelController = null;
    MainMenuHandler menuHandlerClass = null;

    private void Start()
    {
        //sceneLoaderClass = sceneLoader.GetComponent<SceneLoader>();
    }

    public void LoadLevel()
    {
        //sceneLoaderClass.LoadSceneByName("Level1");
        SceneManager.LoadScene("Level1");
    }

    public void LoadChapter()
    {
        if (menuHandlerClass == null)
            menuHandlerClass = FindObjectOfType<MainMenuHandler>().GetComponent<MainMenuHandler>();
        menuHandlerClass.LoadChapter();
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

    public void OnPauseIconClick()
    {
        // Removing this null is giving error which is mapped to levelController from inspector.
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();
        levelController.ClickedPauseButton();
    }

    public void OnResumeClick()
    {
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();
        levelController.ClickedResumeButton();
    }

    public void OnRetryClick()
    {
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();
        levelController.ClickedRetryButton();
    }

    public void LoadNextLevel()
    {

    }

    public void OnTaskCompleted(int index)
    {
        if(menuHandlerClass == null)
            menuHandlerClass = FindObjectOfType<MainMenuHandler>().GetComponent<MainMenuHandler>();
        menuHandlerClass.CompleteTask(index);
    }

    public void LoadUpgradeScene()
    {
        SceneManager.LoadScene("Upgrades");
    }

    public void OnNextChapterArrowClicked()
    {
        if (menuHandlerClass == null)
            menuHandlerClass = FindObjectOfType<MainMenuHandler>().GetComponent<MainMenuHandler>();
        menuHandlerClass.RightArrowClicked();
    }

    public void OnPrevChapterArrowClicked()
    {
        if (menuHandlerClass == null)
            menuHandlerClass = FindObjectOfType<MainMenuHandler>().GetComponent<MainMenuHandler>();
        menuHandlerClass.LeftArrowClicked();
    }

    public void OnLevelSelected()
    {
        var levelClickedGO = EventSystem.current.currentSelectedGameObject;
        string levelName = levelClickedGO.transform.parent.transform.parent.name;
        SceneManager.LoadScene(levelName);
    }

}
