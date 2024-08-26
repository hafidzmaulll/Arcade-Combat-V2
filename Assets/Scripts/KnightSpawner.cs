using UnityEngine;
using Photon.Pun;

public class KnightSpawner : MonoBehaviour
{
    public GameObject knightPrefab;       // Reference to the Knight prefab
    public Transform[] spawnPoints;       // Array of specific spawn points for Knight enemies

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnKnights();
        }
    }

    void SpawnKnights()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            // Ensure Z position is set to 0 for 2D alignment
            Vector3 spawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0);

            // Instantiate the Knight at the specific spawn point's position
            GameObject knightObject = PhotonNetwork.Instantiate(knightPrefab.name, spawnPosition, Quaternion.identity);

            // Debug log to confirm spawning
            Debug.Log("Knight instantiated at position: " + spawnPosition);
        }
    }
}
