using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DoorCodeUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject codePanel;
    public TextMeshProUGUI codeDisplay;
    public TextMeshProUGUI messageText;

    [Header("Key ID")]
    public string requiredKeyId = "key_door_1";

    [Header("Door")]
    public Transform doorTransform;
    public Collider doorCollider;
    public float openAngle = 90f;
    public float animationSpeed = 2f;

    [Header("Settings")]
    public float interactionDistance = 15f;
    public KeyCode interactKey = KeyCode.E;

    private bool isPanelOpen = false;
    private bool isDoorOpen = false;
    private bool isAnimating = false;
    private Transform player;

    void Start()
    {
        codePanel.SetActive(false);
    }

    void Update()
    {
        // Cherche le joueur si pas encore trouvé
        if (player == null)
        {
            GameObject playerObj = 
                GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            return;
        }

        float dist = Vector3.Distance(
            Camera.main.transform.position,
            transform.position
        );

        if (dist < interactionDistance &&
            Input.GetKeyDown(interactKey) &&
            !isAnimating)
        {
            // Vérifier si le joueur a la clé
            if (!PlayerInventory.HasSpecificKey(requiredKeyId))
            {
                ShowHint("");
                return;
            }

            // Le joueur a la clé → ouvrir/fermer
            if (!isDoorOpen)
                StartCoroutine(OpenDoorSequence());
            else
                StartCoroutine(CloseDoorSequence());
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isPanelOpen)
            ClosePanel();
    }

    void ShowHint(string msg)
    {
        if (messageText != null)
        {
            // Annuler l'ancien Invoke si existant
            CancelInvoke("HideHint");
            
            messageText.text = msg;
            messageText.color = Color.yellow;
            codePanel.SetActive(true);
            
            // Attendre 3 secondes avant de cacher
            Invoke("HideHint", 20f);
        }
    }

    void HideHint()
    {
        if (codePanel != null)
            codePanel.SetActive(false);
        if (messageText != null)
            messageText.text = "";
    }

    

    void ShowMessage(string msg, Color color)
    {
        if (messageText != null)
        {
            messageText.text = msg;
            messageText.color = color;
        }
    }

    void TogglePanel()
    {
        isPanelOpen = !isPanelOpen;
        codePanel.SetActive(isPanelOpen);
        Cursor.lockState = isPanelOpen ?
            CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPanelOpen;
    }

    void ClosePanel()
    {
        isPanelOpen = false;
        codePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    IEnumerator OpenDoorSequence()
    {
        isAnimating = true;

        if (doorCollider != null)
            doorCollider.enabled = false;

        float t = 0f;
        Vector3 startRot = doorTransform.localEulerAngles;
        Vector3 endRot = new Vector3(0f, 0f, 90f);

        while (t < 1f)
        {
            t += Time.deltaTime * animationSpeed;
            float smooth = t * t * (3f - 2f * t);
            doorTransform.localEulerAngles = Vector3.Lerp(
                startRot, endRot, smooth
            );
            yield return null;
        }

        isDoorOpen = true;
        isAnimating = false;
    }

    IEnumerator CloseDoorSequence()
    {
        isAnimating = true;

        float t = 0f;
        Vector3 startRot = doorTransform.localEulerAngles;
        Vector3 endRot = new Vector3(0f, 0f, 0f);

        while (t < 1f)
        {
            t += Time.deltaTime * animationSpeed;
            float smooth = t * t * (3f - 2f * t);
            doorTransform.localEulerAngles = Vector3.Lerp(
                startRot, endRot, smooth
            );
            yield return null;
        }

        if (doorCollider != null)
            doorCollider.enabled = true;

        isDoorOpen = false;
        isAnimating = false;
    }
}