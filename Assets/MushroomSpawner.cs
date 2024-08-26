using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MushroomBossSpawner : MonoBehaviour
{
    public GameObject mushroomBossPrefab; // Prefab of the Mushroom boss
    public Transform spawnPoint; // Spawn point for the Mushroom boss

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient) // Ensure only the MasterClient spawns the boss
        {
            SpawnMushroomBoss();
        }
    }

    private void SpawnMushroomBoss()
    {
        PhotonNetwork.Instantiate(mushroomBossPrefab.name, spawnPoint.position, Quaternion.identity);
        // Any additional setup for the Mushroom boss can be done here if needed
    }
}
