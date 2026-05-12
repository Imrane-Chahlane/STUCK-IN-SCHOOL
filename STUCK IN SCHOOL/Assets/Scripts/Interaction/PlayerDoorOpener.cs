using Unity.Netcode;
using UnityEngine;

public class PlayerDoorOpener : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void RequestOpenDoorServerRpc(string doorName)
    {
        OpenDoorClientRpc(doorName);
    }

    [ClientRpc]
    private void OpenDoorClientRpc(string doorName)
    {
        GameObject doorObj = GameObject.Find(doorName);
        if (doorObj != null)
        {
            KeyDoor door = doorObj.GetComponent<KeyDoor>();
            if (door != null)
                door.OpenDoor();
        }
        else
        {
            Debug.LogWarning($"Door '{doorName}' not found!");
        }
    }
}