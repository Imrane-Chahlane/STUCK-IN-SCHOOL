using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : NetworkBehaviour
{
    public static bool keyFound = false;

    // Dictionnaire pour gérer plusieurs clés
    public static Dictionary<string, bool> keys = 
        new Dictionary<string, bool>();

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

    // Nouvelle méthode pour clés spécifiques
    public void GiveSpecificKey(string keyId)
    {
        if (!IsOwner) return;
        keys[keyId] = true;
        SetSpecificKeyServerRpc(keyId);
        Debug.Log("KEY RECEIVED : " + keyId);
    }

    public static bool HasSpecificKey(string keyId)
    {
        return keys.ContainsKey(keyId) && keys[keyId];
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKeyFoundServerRpc()
    {
        SetKeyFoundClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetSpecificKeyServerRpc(string keyId)
    {
        SetSpecificKeyClientRpc(keyId);
    }

    [ClientRpc]
    private void SetKeyFoundClientRpc()
    {
        keyFound = true;
        Debug.Log("Key found synced to all clients!");
    }

    [ClientRpc]
    private void SetSpecificKeyClientRpc(string keyId)
    {
        keys[keyId] = true;
        Debug.Log("Key " + keyId + " synced!");
    }
}