using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;
<<<<<<< HEAD
using System.Collections;

public class PlayerNetwork : NetworkBehaviour
{
    private NetworkVariable<Vector3> networkPosition = 
        new NetworkVariable<Vector3>(
            Vector3.zero,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );

    private NetworkVariable<Quaternion> networkRotation = 
        new NetworkVariable<Quaternion>(
            Quaternion.identity,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );

    public override void OnNetworkSpawn()
    {
        StartCoroutine(SetupPlayer());
    }

    IEnumerator SetupPlayer()
    {
        // Attendre une frame pour que IsOwner soit correct
        yield return null;
        ulong localId = NetworkManager.Singleton.LocalClientId;
        bool isMine = OwnerClientId == localId;

        Debug.Log($"OwnerClientId: {OwnerClientId} | LocalClientId: {localId} | IsMine: {isMine}");

        if (!IsOwner)
        {
            // Désactiver mouvement pour non-owner
            var controller = GetComponentInChildren<StarterAssets.ThirdPersonController>();
            if (controller != null) controller.enabled = false;
=======
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private StarterAssets.ThirdPersonController thirdPersonController;
    [SerializeField] private StarterAssets.StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerInput playerInput;

    private Animator _animator;
    private bool _setupDone = false;

    // NetworkVariables pour sync les animations
    private NetworkVariable<float> _speed = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> _motionSpeed = new NetworkVariable<float>(
        1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> _grounded = new NetworkVariable<bool>(
        true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> _jump = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> _freeFall = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
        if (cameraTarget == null)
        {
            var t = transform.Find("PlayerArmature/PlayerCameraRoot");
            if (t != null) cameraTarget = t;
        }
>>>>>>> 84e5736e14628eb206c0981b82996f7de8777ea0

        TrySetup();
    }

<<<<<<< HEAD
            var cc = GetComponentInChildren<CharacterController>();
            if (cc != null) cc.enabled = false;

            Debug.Log("Non-owner : mouvement désactivé");
            yield break;
        }

        Debug.Log("Owner : je suis le joueur local ✅");

        // Lier la caméra
        GameObject cameraRootObj = GameObject.FindWithTag("CinemachineTarget");
        if (cameraRootObj != null)
        {
            CinemachineCamera vcam = FindAnyObjectByType<CinemachineCamera>();
            if (vcam != null)
            {
                vcam.Target.TrackingTarget = cameraRootObj.transform;
                vcam.Target.LookAtTarget = cameraRootObj.transform;
                Debug.Log("Caméra liée ✅");
            }
=======
    private void Update()
    {
        if (!_setupDone && IsSpawned)
        {
            TrySetup();
            return;
        }

        if (!IsSpawned || _animator == null) return;

        if (IsOwner)
        {
            _speed.Value       = _animator.GetFloat("Speed");
            _motionSpeed.Value = _animator.GetFloat("MotionSpeed");
            _grounded.Value    = _animator.GetBool("Grounded");
            _jump.Value        = _animator.GetBool("Jump");
            _freeFall.Value    = _animator.GetBool("FreeFall");
>>>>>>> 84e5736e14628eb206c0981b82996f7de8777ea0
        }
    }

    private void Update()
    {
        if (!IsSpawned) return;

        if (IsOwner)
        {
            networkPosition.Value = transform.position;
            networkRotation.Value = transform.rotation;
        }
        else
        {
<<<<<<< HEAD
            transform.position = Vector3.Lerp(
                transform.position,
                networkPosition.Value,
                Time.deltaTime * 10f
            );
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                networkRotation.Value,
                Time.deltaTime * 10f
            );
=======
            _animator.SetFloat("Speed",       _speed.Value);
            _animator.SetFloat("MotionSpeed", _motionSpeed.Value);
            _animator.SetBool("Grounded",     _grounded.Value);
            _animator.SetBool("Jump",         _jump.Value);
            _animator.SetBool("FreeFall",     _freeFall.Value);
>>>>>>> 84e5736e14628eb206c0981b82996f7de8777ea0
        }
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

        foreach (var r in GetComponentsInChildren<SkinnedMeshRenderer>())
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

        Debug.Log($"[PlayerNetwork] ✅ Owner activé — ClientId:{OwnerClientId}");
    }

    private void DisableRemote()
    {
        if (playerInput != null)           playerInput.enabled           = false;
        if (thirdPersonController != null) thirdPersonController.enabled = false;
        if (starterAssetsInputs != null)   starterAssetsInputs.enabled   = false;
        if (characterController != null)   characterController.enabled   = false;

        foreach (var r in GetComponentsInChildren<SkinnedMeshRenderer>())
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        Debug.Log($"[PlayerNetwork] Remote désactivé — ClientId:{OwnerClientId}");
    }
}