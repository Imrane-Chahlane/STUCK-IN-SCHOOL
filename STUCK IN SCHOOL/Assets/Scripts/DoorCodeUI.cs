using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Unity.Netcode;

public class DoorCodeUI : NetworkBehaviour
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

    // Synchronise l'état de la porte sur le réseau
    private NetworkVariable<bool> _isDoorOpen = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    void Start()
    {
        // Vérifiez avant de désactiver
        if (codePanel != null)
            codePanel.SetActive(false);
        _isDoorOpen.OnValueChanged += OnDoorStateChanged;
    }

    void OnDoorStateChanged(bool oldValue, bool newValue)
    {
        if (newValue && !isDoorOpen)
            StartCoroutine(OpenDoorSequence());
        else if (!newValue && isDoorOpen)
            StartCoroutine(CloseDoorSequence());
    }

    void Update()
    {
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
            if (!PlayerInventory.HasSpecificKey(requiredKeyId))
            {
                ShowHint("Vous avez besoin de la clé !");
                return;
            }

            // Demander au serveur d'ouvrir/fermer
            if (!_isDoorOpen.Value)
                RequestOpenDoorServerRpc();
            else
                RequestCloseDoorServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isPanelOpen)
            ClosePanel();
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestOpenDoorServerRpc()
    {
        _isDoorOpen.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestCloseDoorServerRpc()
    {
        _isDoorOpen.Value = false;
    }

    void ShowHint(string msg)
    {
        if (messageText != null)
        {
            CancelInvoke("HideHint");
            messageText.text = msg;
            messageText.color = Color.yellow;
            // codePanel.SetActive(true);
            Invoke("HideHint", 5f);
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