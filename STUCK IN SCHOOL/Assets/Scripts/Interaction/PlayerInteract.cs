using Unity.Netcode;
using UnityEngine;

public class PlayerInteract : NetworkBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 60f;
    public GameObject interactUI;
    public Keypad keypad;

    private IInteractable currentInteractable;
    private PlayerInventory playerInventory;

    private void Start()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        if (playerCamera == null)
            playerCamera = Camera.main;

        playerInventory = GetComponent<PlayerInventory>();

        if (interactUI == null)
        {
            GameObject found = GameObject.FindWithTag("InteractUI");
            if (found != null)
                interactUI = found;
        }

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    private void Update()
    {
        if (!IsOwner) return;

        // Fermer le keypad avec Échap
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LockerPuzzle locker = FindFirstObjectByType<LockerPuzzle>();
            if (locker != null)
                locker.CloseKeypad();
        }

        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
            currentInteractable.Interact();
    }

    void CheckForInteractable()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;

                if (interactUI != null)
                    interactUI.SetActive(true);

                Keypad kp = hit.collider.GetComponentInParent<Keypad>();
                if (kp != null && playerInventory != null)
                    kp.SetInventory(playerInventory);

                return;
            }
        }

        currentInteractable = null;

        if (interactUI != null)
            interactUI.SetActive(false);
    }
}