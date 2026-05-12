using UnityEngine;

public class KeyDoor : MonoBehaviour, IInteractable
{
    public Collider doorCollider;
    public Transform doorTransform;
    public Vector3 openOffset = new Vector3(0, 3, 0);
    public Animator animator;

    private bool _isOpen = false;
    private Vector3 _closedPosition;

    private void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (doorCollider == null)
            doorCollider = GetComponentInChildren<Collider>();
        if (doorTransform == null)
            doorTransform = transform;

        _closedPosition = doorTransform.position;
    }

    public void Interact()
    {
        if (PlayerInventory.keyFound)
        {
            PlayerDoorOpener opener = null;
            foreach (var inv in FindObjectsByType<PlayerInventory>(FindObjectsSortMode.None))
            {
                if (inv.IsOwner)
                {
                    opener = inv.GetComponent<PlayerDoorOpener>();
                    break;
                }
            }

            if (opener != null)
                opener.RequestOpenDoorServerRpc(gameObject.name);
        }
        else
        {
            Debug.Log("No key found yet!");
        }
    }

    public void OpenDoor()
    {
        _isOpen = true;
        if (doorCollider != null) doorCollider.enabled = false;
        if (doorTransform != null) doorTransform.position = _closedPosition + openOffset;
        if (animator != null) animator.SetBool("Open", true);
        Debug.Log("Door opened!");
    }
}