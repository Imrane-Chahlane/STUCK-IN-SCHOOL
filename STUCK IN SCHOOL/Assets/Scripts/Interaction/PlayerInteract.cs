using UnityEngine;
using TMPro;
public class PlayerInteract : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 60f;

    public GameObject interactUI; // 👈 UI reference

    private Interactable currentInteractable;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        // Chercher dans tous les Canvas même désactivés
        if (interactUI == null)
        {
            // Chercher par nom
            interactUI = GameObject.Find("IntercatableText");
            
            // Si pas trouvé, chercher dans tous les objets
            if (interactUI == null)
            {
                TextMeshProUGUI[] allTexts = FindObjectsByType<TextMeshProUGUI>(
                    FindObjectsInactive.Include, 
                    FindObjectsSortMode.None);
                foreach (var t in allTexts)
                {
                    if (t.gameObject.name == "IntercatableText")
                    {
                        interactUI = t.gameObject;
                        break;
                    }
                }   
            }
        }
    
        if (interactUI != null)
            interactUI.SetActive(false);
    }
    private void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    void CheckForInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                interactUI.SetActive(true); // 👈 SHOW UI
                return;
            }
        }

        // If nothing hit or no interactable
        currentInteractable = null;
        interactUI.SetActive(false); // 👈 HIDE UI
    }
}