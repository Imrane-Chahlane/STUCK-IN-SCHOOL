using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 5f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed");

            if (playerCamera == null)
            {
                Debug.LogError("Player camera is NULL");
                return;
            }

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                Debug.Log("Hit object: " + hit.collider.gameObject.name);

                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
                else
                {
                    Debug.Log("Hit object has no Interactable script");
                }
            }
            else
            {
                Debug.Log("Nothing hit");
            }
        }
    }
}