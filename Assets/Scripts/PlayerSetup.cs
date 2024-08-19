using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviour
{
    public HealthBar healthBarPrefab;
    private Damageable damageable;

    void Start()
    {
        if (PhotonView.Get(this).IsMine)
        {
            // Find the health bar UI in the scene or instantiate it as needed
            HealthBar healthBar = Instantiate(healthBarPrefab, FindObjectOfType<Canvas>().transform);

            // Initialize the health bar with this player's Damageable component
            damageable = GetComponent<Damageable>();
            healthBar.Initialize(damageable);
        }
    }
}
