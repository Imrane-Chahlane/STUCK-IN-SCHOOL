using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private StarterAssets.ThirdPersonController thirdPersonController;
    [SerializeField] private StarterAssets.StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerInput playerInput;

    private bool _setupDone = false;

    public override void OnNetworkSpawn()
    {
        if (thirdPersonController == null)
            thirdPersonController = GetComponentInChildren<StarterAssets.ThirdPersonController>();
        if (starterAssetsInputs == null)
            starterAssetsInputs = GetComponentInChildren<StarterAssets.StarterAssetsInputs>();
        if (characterController == null)
            characterController = GetComponentInChildren<CharacterController>();
        if (playerInput == null)
            playerInput = GetComponentInChildren<PlayerInput>();
        if (cameraTarget == null)
        {
            var t = transform.Find("PlayerArmature/PlayerCameraRoot");
            if (t != null) cameraTarget = t;
        }

        TrySetup();
    }

    private void Update()
    {
        if (!_setupDone && IsSpawned)
            TrySetup();
    }

    private void TrySetup()
    {
        if (!IsSpawned) return;

        if (OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            _setupDone = true;
            EnableOwner();
        }
        else
        {
            _setupDone = true;
            DisableRemote();
        }
    }

    private void EnableOwner()
    {
        if (playerInput != null)
        {
            playerInput.enabled = true;
            playerInput.neverAutoSwitchControlSchemes = true;
        }
        if (thirdPersonController != null) thirdPersonController.enabled = true;
        if (characterController != null)   characterController.enabled   = true;
        if (starterAssetsInputs != null)   starterAssetsInputs.enabled   = true;

        var vcam = FindAnyObjectByType<CinemachineCamera>();
        if (vcam != null && cameraTarget != null)
        {
            vcam.Target.TrackingTarget = cameraTarget;
            vcam.Target.LookAtTarget   = cameraTarget;
            Debug.Log("[PlayerNetwork] ✅ Caméra liée");
        }

        Debug.Log($"[PlayerNetwork] ✅ Owner activé — ClientId:{OwnerClientId}");
    }

    private void DisableRemote()
    {
        if (playerInput != null)           playerInput.enabled           = false;
        if (thirdPersonController != null) thirdPersonController.enabled = false;
        if (starterAssetsInputs != null)   starterAssetsInputs.enabled   = false;
        if (characterController != null)   characterController.enabled   = false;

        Debug.Log($"[PlayerNetwork] Remote désactivé — ClientId:{OwnerClientId}");
    }
}