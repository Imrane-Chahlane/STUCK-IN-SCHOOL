using UnityEngine;

public class Interact : MonoBehaviour
{
    public float range = 3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, range))
            {
                Debug.Log("Hit: " + hit.collider.name);

                if (hit.collider.CompareTag("Interactable"))
                {
                    Debug.Log("Interacted with " + hit.collider.name);
                }
            }
        }
    }
}