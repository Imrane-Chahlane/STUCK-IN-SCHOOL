using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerNetwork : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            var controller = GetComponentInChildren<StarterAssets.ThirdPersonController>();
            if (controller != null) controller.enabled = false;

            var input = GetComponentInChildren<StarterAssets.StarterAssetsInputs>();
            if (input != null) input.enabled = false;

            return;
        }

        // Chercher PlayerCameraRoot par tag
        GameObject cameraRootObj = GameObject.FindWithTag("CinemachineTarget");

        if (cameraRootObj != null)
        {
            CinemachineCamera vcam = FindAnyObjectByType<CinemachineCamera>();
            if (vcam != null)
            {
                vcam.Target.TrackingTarget = cameraRootObj.transform;
                vcam.Target.LookAtTarget = cameraRootObj.transform;
                Debug.Log("Caméra liée via tag ✅");
            }
        }
        else
        {
            Debug.LogError("CinemachineTarget tag non trouvé !");
        }
    }
}