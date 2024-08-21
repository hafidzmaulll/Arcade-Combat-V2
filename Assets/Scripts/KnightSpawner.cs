using UnityEngine;
using Photon.Pun;

public class KnightSpawner : MonoBehaviour
{
    public GameObject knightPrefab;
    public Transform[] spawnPoints;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient && GameObject.FindObjectsOfType<Knight>().Length == 0)
        {
            SpawnKnight();
        }
    }

    void SpawnKnight()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0); // Set Z to 0
        GameObject knightObject = PhotonNetwork.Instantiate(knightPrefab.name, spawnPosition, Quaternion.identity);
        Debug.Log("Knight instantiated at position: " + spawnPosition);
    }


}
