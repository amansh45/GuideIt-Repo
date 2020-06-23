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
    UpgradeManager upgradeManagerClass = null;

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

    public void OnLoadNextLevelClicked()
    {
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();
        levelController.LoadNextLevel();
    }

    public void OnLevelSelected()
    {
        var levelClickedGO = EventSystem.current.currentSelectedGameObject;
        string levelName = levelClickedGO.transform.parent.transform.parent.name;
        SceneManager.LoadScene(levelName);
    }

    public void ShowPreviewOfUpgrade()
    {
        if (upgradeManagerClass == null)
            upgradeManagerClass = FindObjectOfType<UpgradeManager>();
        var upgradeClickedGO = EventSystem.current.currentSelectedGameObject;
        string upgradeIndex = upgradeClickedGO.transform.parent.name;
        upgradeManagerClass.UpgradeClicked(int.Parse(upgradeIndex));
    }

    public void UpgradeColorClicked()
    {
        if (upgradeManagerClass == null)
            upgradeManagerClass = FindObjectOfType<UpgradeManager>();
        upgradeManagerClass.ShowColorsMenu();
    }
    
    public void EnterToUpgradeMainCanvas()
    {
        if (upgradeManagerClass == null)
            upgradeManagerClass = FindObjectOfType<UpgradeManager>();
        upgradeManagerClass.ShowMainCanvas();
    }

    public void OnSelectColorForUpgrade()
    {
        if (upgradeManagerClass == null)
            upgradeManagerClass = FindObjectOfType<UpgradeManager>();

        var colorClickedGO = EventSystem.current.currentSelectedGameObject;
        string colorName = colorClickedGO.transform.name;

        foreach (Transform child in colorClickedGO.transform.parent)
        {
            if (child.gameObject.name == colorName)
                child.GetChild(1).gameObject.SetActive(true);
            else
                child.GetChild(1).gameObject.SetActive(false);
        }
        upgradeManagerClass.SelectColor(colorName);
    }

    public void BuySkin(bool fromCoin)
    {
        if (upgradeManagerClass == null)
            upgradeManagerClass = FindObjectOfType<UpgradeManager>();

        upgradeManagerClass.BuyButtonClicked(fromCoin);
    }

    public void SelectSkin()
    {
        if (upgradeManagerClass == null)
            upgradeManagerClass = FindObjectOfType<UpgradeManager>();

        upgradeManagerClass.SelectButtonClicked();
    }

}
