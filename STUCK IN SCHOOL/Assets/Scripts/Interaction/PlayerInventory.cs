using Unity.Netcode;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    public static bool keyFound = false;

    private NetworkVariable<bool> _hasKey = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    public bool hasKey => _hasKey.Value;

    public void GiveKey()
    {
        if (!IsOwner) return;

        _hasKey.Value = true;
        keyFound = true;

        SetKeyFoundServerRpc();

        Debug.Log("KEY RECEIVED");

        if (KeyUIManager.Instance != null)
            KeyUIManager.Instance.ShowKeyCollected();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKeyFoundServerRpc()
    {
        SetKeyFoundClientRpc();
    }

    [ClientRpc]
    private void SetKeyFoundClientRpc()
    {
        keyFound = true;
        Debug.Log("Key found synced to all clients!");
    }
}