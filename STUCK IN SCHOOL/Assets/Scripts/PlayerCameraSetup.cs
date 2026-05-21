using Unity.Netcode;
using UnityEngine;

public class PlayerCameraSetup : NetworkBehaviour
{
    public Camera playerCamera;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>(true);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerCamera.gameObject.SetActive(true);
        }
        else
        {
            playerCamera.gameObject.SetActive(false);
        }
    }
}