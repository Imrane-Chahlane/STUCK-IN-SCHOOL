using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public Camera playerCamera;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed");

            if (playerCamera == null)
            {
                Debug.LogWarning("Player camera is not assigned.");
                return;
            }

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                Debug.Log("Hit: " + hit.collider.name);

                Interactable interactable = hit.collider.GetComponent<Interactable>();

                if (interactable != null)
                {
                    interactable.Interact();
                }
                else
                {
                    Debug.Log("This object does not have an Interactable script.");
                }
            }
            else
            {
                Debug.Log("Nothing hit.");
            }
        }
    }
}