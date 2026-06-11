using UnityEngine;
using Unity.Netcode;

public class DoorTrigger2 : MonoBehaviour
{
    public int doorIndex = 1;
    public DoorSplitManager manager;

    private void OnTriggerEnter(Collider other)
    {
        // Cherche NetworkObject dans le parent
        NetworkObject netObj = other.GetComponentInParent<NetworkObject>();
        if (netObj == null) return;
        if (!netObj.IsOwner) return;

        Debug.Log("Joueur entré porte " + doorIndex);
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        manager.PlayerAtDoorServerRpc(doorIndex, clientId);
    }

    private void OnTriggerExit(Collider other)
    {
        NetworkObject netObj = other.GetComponentInParent<NetworkObject>();
        if (netObj == null) return;
        if (!netObj.IsOwner) return;

        Debug.Log("Joueur sorti porte " + doorIndex);
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        manager.PlayerLeftDoorServerRpc(doorIndex, clientId);
    }
}