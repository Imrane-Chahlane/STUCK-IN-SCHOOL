using UnityEngine;

public class KeyDoor : MonoBehaviour, IInteractable
{
    public Animator animator;

    private bool isOpen = false;

    public void Interact()
    {
        Debug.Log("DOOR INTERACT CALLED");

        PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogError("No PlayerInventory found!");
            return;
        }

        if (inventory.hasKey)
        {
            isOpen = !isOpen;

            animator.SetBool("Open", isOpen);

            Debug.Log("Door opened");
        }
        else
        {
            Debug.Log("Door is locked. Need key.");
        }
    }
}