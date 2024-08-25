using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SceneTransitionManager : MonoBehaviourPunCallbacks
{
    public string mainMenuSceneName = "MainMenu";

    public void GoToMainMenu()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            // If not in a room, directly go to the main menu
            LoadMainMenuScene();
        }
    }

    public override void OnLeftRoom()
    {
        // This callback is called when the player leaves the room
        LoadMainMenuScene();
    }

    private void LoadMainMenuScene()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
