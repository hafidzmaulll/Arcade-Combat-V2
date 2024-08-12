using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Player"))
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            player.InstantDeath();
        }
    } else if (collision.CompareTag("Enemy"))
    {
        Knight enemy = collision.GetComponent<Knight>();
        if (enemy != null)
        {
            enemy.InstantDeath();
        }
    }
}
}
