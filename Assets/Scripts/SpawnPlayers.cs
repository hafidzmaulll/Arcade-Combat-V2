using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    private void Start()
    {
        // Instantiate the player at a random position
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(Random.Range(-2, 2), 1f, 0), Quaternion.identity);

        // Disable the Mark GameObject in the player's PlayerController component
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null && playerController.Mark != null)
            {
                playerController.Mark.SetActive(true);
            }
        }
    }
}
