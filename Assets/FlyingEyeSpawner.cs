using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class FlyingEyeSpawner : MonoBehaviour
{
    public GameObject flyingEyePrefab;   // Reference to the Flying Eye prefab
    public Transform[] spawnPoints;      // Array of possible spawn points for Flying Eye enemies
    public List<Transform> waypointSet1; // Example waypoint set for Flying Eye
    public List<Transform> waypointSet2; // Another waypoint set for Flying Eye

    void Start()
    {
        // Only the Master Client should spawn the enemies
        if (PhotonNetwork.IsMasterClient && GameObject.FindObjectsOfType<FlyingEye>().Length == 0)
        {
            SpawnFlyingEye();
        }
    }

    void SpawnFlyingEye()
    {
        // Choose a random spawn point from the array
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instantiate the Flying Eye at the chosen spawn point
        GameObject flyingEyeObject = PhotonNetwork.Instantiate(flyingEyePrefab.name, spawnPoint.position, Quaternion.identity);

        // Get the FlyingEye component from the instantiated object
        FlyingEye flyingEye = flyingEyeObject.GetComponent<FlyingEye>();

        // Assign waypoints dynamically (you can choose which waypoint set to use based on your logic)
        flyingEye.SetWaypoints(waypointSet1); // You can replace with waypointSet2 or any other logic
    }
}
