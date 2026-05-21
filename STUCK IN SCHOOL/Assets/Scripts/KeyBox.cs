
using UnityEngine;
using TMPro;
using System.Collections;
using Unity.Netcode;

public class KeyBox : NetworkBehaviour
{
    [Header("Code")]
    public string correctCode = "54321";

    [Header("Key ID")]
    public string keyId = "key_door_1";

    [Header("UI")]
    public GameObject codePanel;
    public TextMeshProUGUI codeDisplay;
    public TextMeshProUGUI messageText;

    [Header("Animation")]
    public Animation chestAnimation;
    public string openClipName = "Scene";

    [Header("Manuscript")]
    public GameObject manuscript;
    public float manuscriptDuration = 10f;

    [Header("Settings")]
    public float interactionDistance = 15f;
    public KeyCode interactKey = KeyCode.E;


    private string currentInput = "";
    private bool isPanelOpen = false;
    private bool isUnlocked = false;
    private PlayerInventory localPlayer;

    private NetworkVariable<bool> _isUnlocked = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    void Start()
    {
        if (codePanel != null)
            codePanel.SetActive(false);
        _isUnlocked.OnValueChanged += OnUnlockedChanged;
    }

    void OnUnlockedChanged(bool oldValue, bool newValue)
    {
        if (newValue && !isUnlocked)
        {
            isUnlocked = true;
            StartCoroutine(PlayAnimationThenShowMessage());
        }
    }

    void Update()
    {
        if (localPlayer == null)
        {
            foreach (var p in FindObjectsOfType<PlayerInventory>())
            {
                if (p.IsOwner)
                {
                    localPlayer = p;
                    break;
                }
            }
            return;
        }

        if (isUnlocked) return;

        float dist = Vector3.Distance(
            Camera.main.transform.position,
            transform.position
        );

        if (dist < interactionDistance &&
            Input.GetKeyDown(interactKey))
        {
            TogglePanel();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isPanelOpen)
            ClosePanel();
    }

    public void Press0() { AddNumber("0"); }
    public void Press1() { AddNumber("1"); }
    public void Press2() { AddNumber("2"); }
    public void Press3() { AddNumber("3"); }
    public void Press4() { AddNumber("4"); }
    public void Press5() { AddNumber("5"); }
    public void Press6() { AddNumber("6"); }
    public void Press7() { AddNumber("7"); }
    public void Press8() { AddNumber("8"); }
    public void Press9() { AddNumber("9"); }

    private void AddNumber(string number)
    {
        if (currentInput.Length < 5)
        {
            currentInput += number;
            UpdateDisplay();
            if (messageText != null)
                messageText.text = "";
        }
    }

    public void PressDelete()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(
                0, currentInput.Length - 1);
            UpdateDisplay();
        }
    }

    public void PressConfirm()
    {
        if (currentInput.Length == 0)
        {
            ShowMessage("Entrez un code !", Color.yellow);
            return;
        }

        if (currentInput == correctCode)
        {
            ShowMessage("Code correct !", Color.green);
            RequestUnlockServerRpc();
        }
        else
        {
            ShowMessage("Code incorrect !", Color.red);
            currentInput = "";
            UpdateDisplay();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestUnlockServerRpc()
    {
        _isUnlocked.Value = true;
        GiveKeyToAllClientRpc();
    }

    [ClientRpc]
    void GiveKeyToAllClientRpc()
    {
        foreach (var p in FindObjectsOfType<PlayerInventory>())
        {
            if (p.IsOwner)
            {
                p.GiveSpecificKey(keyId);
                break;
            }
        }
    }

    IEnumerator PlayAnimationThenShowMessage()
    {
        Invoke("ClosePanel", 0.5f);

        if (chestAnimation != null)
        {
            chestAnimation[openClipName].wrapMode = WrapMode.Once;
            chestAnimation.Play(openClipName);
            yield return new WaitForSeconds(
                chestAnimation[openClipName].length
            );
        }

        if (manuscript != null)
        {
            manuscript.SetActive(true);
            yield return new WaitForSeconds(manuscriptDuration);
            manuscript.SetActive(false);
        }
    }

    public void PressCancel()
    {
        ClosePanel();
    }

    void TogglePanel()
    {
        isPanelOpen = !isPanelOpen;
        codePanel.SetActive(isPanelOpen);
        Cursor.lockState = isPanelOpen ?
            CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPanelOpen;

        if (!isPanelOpen)
        {
            currentInput = "";
            UpdateDisplay();
            if (messageText != null)
                messageText.text = "";
        }
    }

    void ClosePanel()
    {
        isPanelOpen = false;
        codePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentInput = "";
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (codeDisplay == null) return;
        string display = "";
        for (int i = 0; i < currentInput.Length; i++)
            display += "● ";
        codeDisplay.text = display;
    }

    void ShowMessage(string msg, Color color)
    {
        if (messageText == null) return;
        messageText.text = msg;
        messageText.color = color;
    }
}