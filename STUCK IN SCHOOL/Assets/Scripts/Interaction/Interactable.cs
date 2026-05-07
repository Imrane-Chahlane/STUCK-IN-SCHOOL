using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string objectMessage = "Interaction works";

    public Animator doorAnimator;

    private bool isOpen = false;

    public void Interact()
    {
        Debug.Log("Interacted with: " + gameObject.name);
        Debug.Log(objectMessage);
        
        isOpen = !isOpen;

        doorAnimator.SetBool("Open", isOpen);
    }
}