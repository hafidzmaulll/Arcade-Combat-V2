using UnityEngine;
using Photon.Pun;

public class SpikeTrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null && player.photonView.IsMine)
            {
                player.photonView.RPC("InstantDeath", RpcTarget.All);
            }
        }
        else if (collision.CompareTag("Enemy"))
        {
            Knight enemy = collision.GetComponent<Knight>();
            if (enemy != null && enemy.photonView.IsMine)
            {
                enemy.photonView.RPC("InstantDeath", RpcTarget.All);
            }
        }
    }
}
