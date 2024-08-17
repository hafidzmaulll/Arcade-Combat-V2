using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;

    public void CreateRoom()
    {
        if (!string.IsNullOrWhiteSpace(createInput.text))
        {
            PhotonNetwork.CreateRoom(createInput.text);
        }
        else
        {
            Debug.LogWarning("Room name cannot be empty!");
            // Optionally, you can display a message to the user using TextMeshPro or another UI element
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
            Debug.LogWarning("Room name cannot be empty!");
            // Optionally, you can display a message to the user using TextMeshPro or another UI element
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameplayScene");
    }
}
