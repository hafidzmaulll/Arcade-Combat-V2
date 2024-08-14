using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject projectilePrefab;

    public void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, projectilePrefab.transform.rotation);
        Vector3 origScale = projectile.transform.localScale;

        // Flip the projectile's facing direction and movement based on the direction the character is facing at time of lunch
        projectile.transform.localScale = new Vector3(
            origScale.x * transform.localScale.x > 0 ? 1 : -1,
            origScale.y,
            origScale.z
        );

        bool isFacingRight = transform.localScale.x > 0;
    
        // Set the projectile's rotation based on the direction
        projectile.transform.eulerAngles = new Vector3(
            0, 
            0, 
            isFacingRight ? 225 : 135 // 0 degrees for right, 180 degrees for left
    );
    }
}

