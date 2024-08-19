using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class PlayerCameraSetup : MonoBehaviour
{
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            // Find or create the Cinemachine Virtual Camera
            CinemachineVirtualCamera vCam = FindObjectOfType<CinemachineVirtualCamera>();

            if (vCam == null)
            {
                GameObject vCamGameObject = new GameObject("PlayerCamera");
                vCam = vCamGameObject.AddComponent<CinemachineVirtualCamera>();
            }

            // Set the target to this player
            vCam.Follow = transform;
            vCam.LookAt = transform;
        }
    }
}
