using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    private AsyncLoader asyncLoader;
    private string gameMode;

    void Start()
    {
        // Find the AsyncLoader in the scene and get the game mode
        asyncLoader = FindObjectOfType<AsyncLoader>();
        if (asyncLoader != null)
        {
            gameMode = asyncLoader.GetGameMode();
        }

        PhotonNetwork.ConnectUsingSettings();  // Connect to Photon for both Singleplayer and Multiplayer
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        if (gameMode == "Singleplayer")
        {
            // Automatically create a room when in Singleplayer mode
            PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 1 });
        }
        else
        {
            // Load the Lobby scene for multiplayer
            SceneManager.LoadScene("Lobby");
        }
    }

    public override void OnCreatedRoom()
    {
        if (gameMode == "Singleplayer")
        {
            // Automatically load Level 1 when the room is created in Singleplayer mode
            PhotonNetwork.LoadLevel("Level1");
        }
    }

    public override void OnJoinedRoom()
    {
        if (gameMode != "Singleplayer")
        {
            // For multiplayer, when the room is joined, do nothing extra (or add custom logic here)
        }
    }
}
