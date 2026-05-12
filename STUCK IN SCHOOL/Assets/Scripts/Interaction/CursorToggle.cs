using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;

public class CursorToggle : MonoBehaviour
{
    private bool _cursorVisible = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _cursorVisible = !_cursorVisible;
            SetCursorState(_cursorVisible);
        }
    }

    private void SetCursorState(bool visible)
    {
        if (visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Trouver le joueur local et désactiver ses inputs
        foreach (var inv in FindObjectsByType<PlayerInventory>(FindObjectsSortMode.None))
        {
            if (inv.IsOwner)
            {
                // Désactiver ThirdPersonController
                var tpc = inv.GetComponentInChildren<ThirdPersonController>();
                if (tpc != null) tpc.enabled = !visible;

                // Désactiver StarterAssetsInputs
                var sai = inv.GetComponentInChildren<StarterAssetsInputs>();
                if (sai != null) sai.enabled = !visible;

                // Désactiver PlayerInput
                var pi = inv.GetComponentInChildren<PlayerInput>();
                if (pi != null) pi.enabled = !visible;

                break;
            }
        }
    }
}