using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;
    public TextMeshProUGUI feedbackText; // UI element to display warnings

    void Start()
    {
        // Ensure the feedback text is hidden initially
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrWhiteSpace(createInput.text))
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2; // Limit the number of players to 2
            PhotonNetwork.CreateRoom(createInput.text, roomOptions, null);
        }
        else
        {
            DisplayWarning("Room name cannot be empty!");
        }
    }

    public void JoinRoom()
    {
        if (!string.IsNullOrWhiteSpace(joinInput.text))
        {
            PhotonNetwork.JoinRoom(joinInput.text);
        }
        else
        {
            DisplayWarning("Room name cannot be empty!");
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Level1");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        DisplayWarning("Failed to join the room: " + message);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        DisplayWarning("Failed to create the room: " + message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; // Close the room when it's full
            DisplayWarning("The lobby is full!");
        }
    }

    private void DisplayWarning(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning(message);
        }
    }
}
