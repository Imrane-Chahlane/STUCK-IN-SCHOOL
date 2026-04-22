using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string objectMessage = "Interaction works";

    public virtual void Interact()
    {
        Debug.Log("Interacted with: " + gameObject.name);
        Debug.Log(objectMessage);
    }
}
//public virtual void Interact()