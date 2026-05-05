// PlayerInteract.cs
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 40f;

    public GameObject interactUI; // 👈 UI reference

    private Interactable currentInteractable;

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
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                interactUI.SetActive(true); // 👈 SHOW UI
                return;
            }
        }

        // If nothing hit or no interactable
        currentInteractable = null;
        interactUI.SetActive(false); // 👈 HIDE UI
    }
}