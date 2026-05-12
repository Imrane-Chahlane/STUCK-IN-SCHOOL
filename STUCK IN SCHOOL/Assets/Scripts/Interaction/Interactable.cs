using Unity.Netcode;
using UnityEngine;

public class Interactable : NetworkBehaviour, IInteractable
{
    public string objectMessage = "Interaction works";
    public Animator doorAnimator;

    private NetworkVariable<bool> _isOpen = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        _isOpen.OnValueChanged += (oldValue, newValue) =>
        {
            if (doorAnimator != null)
                doorAnimator.SetBool("Open", newValue);
        };
    }

    public void Interact()
    {
        Debug.Log("Interacted with: " + gameObject.name);
        ToggleDoorServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleDoorServerRpc()
    {
        _isOpen.Value = !_isOpen.Value;
    }
}