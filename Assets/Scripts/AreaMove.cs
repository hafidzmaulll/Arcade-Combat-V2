using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AreaMove : MonoBehaviourPunCallbacks
{
    public Vector3 teleportDestination;  // Set the destination coordinates in the Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView photonView = other.GetComponent<PhotonView>();

            if (photonView != null && photonView.IsMine)
            {
                PlayerController playerController = other.GetComponent<PlayerController>();
                
                if (playerController != null)
                {
                    // Teleport the player to the destination
                    playerController.Teleport(teleportDestination);
                }
                else
                {
                    Debug.LogError("PlayerController script not found on the player object.");
                }
            }
        }
    }
}
