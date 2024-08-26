using UnityEngine;

public class CanvasUsername : MonoBehaviour
{
    private Transform cameraTransform;

    void Start()
    {
        // Cache the camera's transform
        cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Make sure the canvas parent (UsernameCanvasParent) always faces the camera
        Vector3 direction = cameraTransform.position - transform.position;
        direction.x = direction.z = 0;  // Keep the canvas upright, only rotating on the Y axis

        // Apply rotation to make the canvas face the camera
        transform.LookAt(cameraTransform.position - direction);
    }
}
