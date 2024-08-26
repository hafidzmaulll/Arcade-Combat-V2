using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerUsernameDisplay : MonoBehaviour
{
    public TextMeshProUGUI usernameText; // Assign in the Inspector
    private Camera mainCamera;
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        
        if (photonView.IsMine)
        {
            // Assign the player's username if it's the local player
            usernameText.text = PhotonNetwork.NickName;
        }
        else
        {
            // For remote players, get the username from the PhotonView owner
            usernameText.text = photonView.Owner.NickName;
        }

        // Find the main camera in the scene
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // Make the Canvas face the camera
        if (mainCamera != null)
        {
            usernameText.transform.LookAt(mainCamera.transform);
            usernameText.transform.Rotate(0, 180, 0); // Adjust rotation to face the correct direction
        }
    }
}
