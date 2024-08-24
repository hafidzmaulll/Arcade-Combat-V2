using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class AreaMove : MonoBehaviourPunCallbacks
{
    public int sceneBuildIndex;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView photonView = other.GetComponent<PhotonView>();

            if (photonView != null && photonView.IsMine)
            {
                // Store the current player's position to respawn at the correct location in the new scene
                Vector3 playerPosition = other.transform.position;

                // Switch scene and pass the player's position as data
                PhotonNetwork.LoadLevel(sceneBuildIndex);
                StartCoroutine(RespawnPlayer(photonView.ViewID, playerPosition));
            }
        }
    }

    private IEnumerator RespawnPlayer(int viewID, Vector3 position)
    {
        // Wait for the new scene to load
        while (PhotonNetwork.LevelLoadingProgress < 1f)
        {
            yield return null;
        }

        // Respawn the player in the new scene at the stored position
        GameObject player = PhotonView.Find(viewID).gameObject;
        if (player != null)
        {
            player.transform.position = position;
        }

        // Ensure enemies and items are properly instantiated and synchronized
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("KnightEnemy", Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate("FlyingEye", Vector3.zero, Quaternion.identity);
        }

        // Instantiate and synchronize any items like healing items
        // Example:
        // PhotonNetwork.Instantiate("HealingItem", itemPosition, Quaternion.identity);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // Ensure player is instantiated on joining a new scene
        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        // Handle cleanup or special cases when a player leaves the room
    }
}