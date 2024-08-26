using UnityEngine;
using TMPro;
using Photon.Pun;

public class AssignCameraToCanvas : MonoBehaviour
{
    public Canvas playerCanvas;
    public TextMeshProUGUI usernameText;
    
    void Start()
    {
        // Assign the main camera to the canvas
        playerCanvas.worldCamera = Camera.main;

        // Display the player's username
        if (PhotonNetwork.LocalPlayer != null)
        {
            usernameText.text = PhotonNetwork.LocalPlayer.NickName;
        }
    }
}
