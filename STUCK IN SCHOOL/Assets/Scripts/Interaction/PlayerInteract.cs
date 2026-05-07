using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 60f;

    public GameObject interactUI;

    public Keypad keypad; // 🔐 for linking inventory

    private IInteractable currentInteractable;
    private PlayerInventory playerInventory;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        // Get player inventory (important for multiplayer-ready design)
        playerInventory = GetComponent<PlayerInventory>();

        // Auto find UI if not assigned
        if (interactUI == null)
        {
            GameObject found = GameObject.FindWithTag("InteractUI");
            if (found != null)
                interactUI = found;
        }

        // UI MUST start hidden
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    private void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    void CheckForInteractable()
    {
        Debug.Log("UI ACTIVE STATE: " + interactUI.activeSelf);

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            Debug.Log("Hit: " + hit.collider.name);

            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                Debug.Log("INTERACTABLE FOUND");

                currentInteractable = interactable;

                if (interactUI != null)
                    interactUI.SetActive(true);

                // 🔐 KEYPAD HANDLING
                Keypad kp = hit.collider.GetComponentInParent<Keypad>();

                if (kp != null && playerInventory != null)
                {
                    kp.SetInventory(playerInventory);
                }

                return;
            }
        }

        Debug.Log("NO INTERACTABLE");

        currentInteractable = null;

        if (interactUI != null)
            interactUI.SetActive(false);
    }
}