using UnityEngine;
using UnityEngine.SceneManagement;

public class BossDefeatedMenu : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ProceedToNextLevel()
    {
        // Assuming you have a way to determine the next level
        SceneManager.LoadScene("NextLevelName");
    }
}
