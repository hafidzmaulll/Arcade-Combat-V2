using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AsyncLoader : MonoBehaviour
{
    [Header("Menu Screens")]
    [SerializeField] private GameObject loadingScene;
    [SerializeField] private GameObject mainMenu;

    [Header("Slider")]
    [SerializeField] private Slider loadSlider;

    private string gameMode; // To store the selected game mode

    public void LoadLevelBtn(string levelToLoad)
    {
        mainMenu.SetActive(false);
        loadingScene.SetActive(true);

        // Set the game mode based on the button clicked
        gameMode = levelToLoad;
        
        StartCoroutine(LoadLevelAsync("LoadingScene")); // Always load the loading screen first
    }

    IEnumerator LoadLevelAsync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);

        while (!loadOperation.isDone) 
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadSlider.value = progressValue;
            yield return null;
        }
    }

    // This method will be called by the loading screen to get the game mode
    public string GetGameMode()
    {
        return gameMode;
    }
}
