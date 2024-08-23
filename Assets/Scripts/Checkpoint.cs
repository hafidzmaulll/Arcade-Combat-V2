using UnityEngine;
using Photon.Pun;

public class Checkpoint : MonoBehaviourPun
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null && player.photonView.IsMine) // Ensure the checkpoint only affects the local player
            {
                player.SetCheckpoint(this);
            }
        }
    }
}
