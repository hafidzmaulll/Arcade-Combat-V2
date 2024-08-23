using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class FlyingEyeSpawner : MonoBehaviour
{
    public GameObject flyingEyePrefab;   // Reference to the Flying Eye prefab
    public Transform[] spawnPoints;      // Array of specific spawn points for Flying Eye enemies
    public List<Transform> waypointSet1; // Example waypoint set for Flying Eye
    public List<Transform> waypointSet2; // Another waypoint set for Flying Eye

    void Start()
    {
        // Only the Master Client should spawn the enemies
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnFlyingEyes();
        }
    }

    void SpawnFlyingEyes()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            // Ensure Z position is set to 0 for 2D alignment
            Vector3 spawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0);

            // Instantiate the Flying Eye at the specific spawn point's position
            GameObject flyingEyeObject = PhotonNetwork.Instantiate(flyingEyePrefab.name, spawnPosition, Quaternion.identity);

            // Get the FlyingEye component from the instantiated object
            FlyingEye flyingEye = flyingEyeObject.GetComponent<FlyingEye>();

            // Assign waypoints dynamically (you can choose which waypoint set to use based on your logic)
            flyingEye.SetWaypoints(waypointSet1); // You can replace with waypointSet2 or any other logic

            // Debug log to confirm spawning
            Debug.Log("Flying Eye instantiated at position: " + spawnPosition);
        }
    }
}
