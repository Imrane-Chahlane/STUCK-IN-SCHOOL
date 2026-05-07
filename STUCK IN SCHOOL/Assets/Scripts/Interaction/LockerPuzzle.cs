using UnityEngine;

public class LockerPuzzle : MonoBehaviour, IInteractable
{
    public GameObject keypadUI;

    public void Interact()
    {
        keypadUI.SetActive(true);
        // Debug.Log("Locker opened keypad");
    }
}