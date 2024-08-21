using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthPickup : MonoBehaviourPun
{
    public float healthRestore = 20;
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if(damageable)
        {
            bool wasHealed = damageable.Heal(healthRestore);

            if(wasHealed)
            {
                // Use an RPC to ensure all clients destroy the object
                photonView.RPC("PickupHealth", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void PickupHealth()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
    }
}
