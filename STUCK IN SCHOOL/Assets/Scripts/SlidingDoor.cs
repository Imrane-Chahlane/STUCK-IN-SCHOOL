using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public Animator animator;

    private bool isPlayerNear = false;
    private bool isOpen = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
            animator.SetBool("Open", isOpen);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}