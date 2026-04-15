using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string objectMessage = "Interaction works";

    public void Interact()
    {
        Debug.Log("Interacted with: " + gameObject.name);
        Debug.Log(objectMessage);
    }
}