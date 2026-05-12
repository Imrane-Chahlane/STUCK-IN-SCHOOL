using Unity.Netcode;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;

public class LockerPuzzle : NetworkBehaviour, IInteractable
{
    public GameObject keypadUI;

    public void Interact()
    {
        keypadUI.SetActive(true);
        SetPlayerState(false);
    }

    public void CloseKeypad()
    {
        keypadUI.SetActive(false);
        SetPlayerState(true);
    }

    private void SetPlayerState(bool active)
    {
        Cursor.lockState = active ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !active;

        foreach (var inv in FindObjectsByType<PlayerInventory>(FindObjectsSortMode.None))
        {
            if (inv.IsOwner)
            {
                var tpc = inv.GetComponentInChildren<ThirdPersonController>();
                if (tpc != null) tpc.enabled = active;

                var sai = inv.GetComponentInChildren<StarterAssetsInputs>();
                if (sai != null) sai.enabled = active;

                var pi = inv.GetComponentInChildren<PlayerInput>();
                if (pi != null) pi.enabled = active;

                break;
            }
        }
    }
}