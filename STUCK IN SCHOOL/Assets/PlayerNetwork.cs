using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;
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

            var input = GetComponentInChildren<StarterAssets.StarterAssetsInputs>();
            if (input != null) input.enabled = false;

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
        }
    }
}