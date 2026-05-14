using UnityEngine;

public class LockerPuzzle : MonoBehaviour, IInteractable
{
    public GameObject keypadUI;

    private bool isOpen = false;

    public void Interact()
    {
        isOpen = !isOpen;

        keypadUI.SetActive(isOpen);

        if (isOpen)
        {
            // Ouvre le panel → libère la souris pour cliquer les boutons
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Ferme le panel → recapture la souris pour bouger le joueur
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}