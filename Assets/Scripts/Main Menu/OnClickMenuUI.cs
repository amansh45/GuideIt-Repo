using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickMenuUI : MonoBehaviour
{

    //[SerializeField] GameObject sceneLoader;
    //SceneLoader sceneLoaderClass;

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
}
