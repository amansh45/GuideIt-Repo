using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

// onTaskCompleted sfx pending


public class OnClickMenuUI : MonoBehaviour
{
    //[SerializeField] GameObject sceneLoader;
    [SerializeField] AudioClip buttonClick;
    [SerializeField] AudioClip wooshSFX;
    [SerializeField] AudioClip taskCompletedSFX;

    SceneLoader sceneLoaderClass = null;
    LevelController levelController = null;
    MainMenuHandler menuHandlerClass = null;
    UpgradeManager upgradeManagerClass = null;
    LevelComplete levelComplete = null;
    AdMob adMob = null;

    private void Start()
    {
        sceneLoaderClass = FindObjectOfType<SceneLoader>();
    }

    private void PlaySound(AudioClip clipToBePlayed)
    {
        PlayerStatistics playerStats = FindObjectOfType<PlayerStatistics>();
        AudioSource.PlayClipAtPoint(clipToBePlayed, Camera.main.transform.position, playerStats.sfxVolume);
    }

    public void LoadChapter()
    {
        PlaySound(wooshSFX);
        if (menuHandlerClass == null)
            menuHandlerClass = FindObjectOfType<MainMenuHandler>().GetComponent<MainMenuHandler>();
        menuHandlerClass.LoadChapter();
    }

    public void LoadMainMenu()
    {
        PlaySound(wooshSFX);

        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();

        if (levelController != null)
            levelController.ExecuteResetTaskRequest();

        if (sceneLoaderClass == null)
            sceneLoaderClass = FindObjectOfType<SceneLoader>();
        sceneLoaderClass.LoadScene("Main Menu");
    }

    public void LoadSettings()
    {
        PlaySound(wooshSFX);

        if (sceneLoaderClass == null)
            sceneLoaderClass = FindObjectOfType<SceneLoader>();
       
        sceneLoaderClass.LoadScene("Settings");
    }

    public void OnPauseIconClick()
    {
        PlaySound(buttonClick);
        // Removing this null is giving error which is mapped to levelController from inspector.
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();
        levelController.ClickedPauseButton();
    }

    public void OnWatchAdForResumeLevelClick()
    {
        PlaySound(buttonClick);
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();
        levelController.ResumeLevelAfterWatchingAd();
    }

    public void OnResumeClick()
    {
        PlaySound(buttonClick);
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();
        levelController.ClickedResumeButton();
    }

    public void OnRetryClick()
    {
        PlaySound(buttonClick);
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();
        levelController.ClickedRetryButton(true);
    }
    

    public void OnTaskCompleted(int index)
    {
        PlaySound(taskCompletedSFX);
        if(menuHandlerClass == null)
            menuHandlerClass = FindObjectOfType<MainMenuHandler>().GetComponent<MainMenuHandler>();
        menuHandlerClass.CompleteTask(index);
    }

    public void LoadUpgradeScene()
    {
        PlaySound(wooshSFX);
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();

        if (levelController != null)
            levelController.ExecuteResetTaskRequest();

        if (sceneLoaderClass == null)
            sceneLoaderClass = FindObjectOfType<SceneLoader>();
        sceneLoaderClass.LoadScene("Upgrades");
    }

    public void OnNextChapterArrowClicked()
    {
        PlaySound(buttonClick);
        if (menuHandlerClass == null)
            menuHandlerClass = FindObjectOfType<MainMenuHandler>().GetComponent<MainMenuHandler>();
        menuHandlerClass.RightArrowClicked();
    }

    public void OnPrevChapterArrowClicked()
    {
        PlaySound(buttonClick);
        if (menuHandlerClass == null)
            menuHandlerClass = FindObjectOfType<MainMenuHandler>().GetComponent<MainMenuHandler>();
        menuHandlerClass.LeftArrowClicked();
    }

    public void OnLoadNextLevelClicked()
    {
        PlaySound(buttonClick);
        if (levelComplete == null)
            levelComplete = FindObjectOfType<LevelComplete>();
        levelComplete.LoadNextLevel();
    }

    public void OnClickRetryAfterLevelComplete()
    {
        PlaySound(buttonClick);
        if (levelComplete == null)
            levelComplete = FindObjectOfType<LevelComplete>();
        levelComplete.RetryCurrentLevel();
    }

    public void OnLevelSelected()
    {
        PlaySound(wooshSFX);
        var levelClickedGO = EventSystem.current.currentSelectedGameObject;
        string levelName = levelClickedGO.transform.parent.transform.parent.name;
        SceneManager.LoadScene(levelName);
    }

    public void ShowPreviewOfUpgrade()
    {
        PlaySound(buttonClick);
        if (upgradeManagerClass == null)
            upgradeManagerClass = FindObjectOfType<UpgradeManager>();
        var upgradeClickedGO = EventSystem.current.currentSelectedGameObject;
        string upgradeIndex = upgradeClickedGO.transform.parent.name;
        upgradeManagerClass.UpgradeClicked(int.Parse(upgradeIndex));
    }

    public void UpgradeColorClicked()
    {
        PlaySound(buttonClick);
        if (upgradeManagerClass == null)
            upgradeManagerClass = FindObjectOfType<UpgradeManager>();
        upgradeManagerClass.ShowColorsMenu();
    }
    
    public void EnterToUpgradeMainCanvas()
    {
        PlaySound(buttonClick);
        if (upgradeManagerClass == null)
            upgradeManagerClass = FindObjectOfType<UpgradeManager>();
        upgradeManagerClass.ShowMainCanvas();
    }

    public void OnSelectColorForUpgrade()
    {
        PlaySound(buttonClick);
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
        PlaySound(buttonClick);
        if (upgradeManagerClass == null)
            upgradeManagerClass = FindObjectOfType<UpgradeManager>();

        upgradeManagerClass.SelectButtonClicked();
    }


    public void OnClickRequestAd()
    {
        adMob = FindObjectOfType<AdMob>();
        adMob.RequestRewardBasedVideo();
    }

    public void OnClickShowAd()
    {
        adMob = FindObjectOfType<AdMob>();
        adMob.ShowVideoRewardedAd();
    }

}
