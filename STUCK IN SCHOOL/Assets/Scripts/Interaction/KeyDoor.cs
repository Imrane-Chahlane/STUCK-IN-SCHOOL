using UnityEngine;

public class KeyDoor : MonoBehaviour, IInteractable
{
    public Animator animator; // PUERTA_1 déjà branché ✅

    private bool isOpen = false;

    // Appelé quand le joueur interagit avec la porte manuellement
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
            ForceOpen(); // utilise la même méthode
        }
        else
        {
            Debug.Log("Door is locked. Need key.");
        }
    }

    // ──────────────────────────────────────────
    // MÉTHODE AJOUTÉE : appelée par Keypad.cs
    public void ForceOpen()
    {
        if (isOpen) return; // déjà ouverte, on ne fait rien

        isOpen = true;
        animator.SetBool("Open", true);
        GetComponent<Collider>().enabled = false;
        Debug.Log("Door opened!");
    }
    // ──────────────────────────────────────────
}