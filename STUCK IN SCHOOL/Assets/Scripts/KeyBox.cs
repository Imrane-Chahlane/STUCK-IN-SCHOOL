using UnityEngine;
using TMPro;
using System.Collections;

public class KeyBox : MonoBehaviour
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

    void Start()
    {
        if (codePanel != null)
            codePanel.SetActive(false);
    }

    void Update()
    {
        // Cherche le joueur local
        if (localPlayer == null)
        {
            foreach (var p in
                FindObjectsOfType<PlayerInventory>())
            {
                if (p.IsOwner)
                {
                    localPlayer = p;
                    break;
                }
            }
            return;
        }

        // Si déjà déverrouillé on ne fait rien
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

    // === BOUTONS CHIFFRES ===
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
            // Effacer message erreur
            if (messageText != null)
                messageText.text = "";
        }
    }

    // === BOUTON SUPPRIMER ===
    public void PressDelete()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(
                0, currentInput.Length - 1);
            UpdateDisplay();
        }
    }

    // === BOUTON CONFIRMER ===
    // === BOUTON CONFIRMER ===
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
            isUnlocked = true;
            StartCoroutine(PlayAnimationThenShowMessage());
        }
        else
        {
            ShowMessage("Code incorrect !", Color.red);
            currentInput = "";
            UpdateDisplay();
        }
    }

    IEnumerator PlayAnimationThenShowMessage()
    {
        Invoke("ClosePanel", 0.5f);
        // Animation chest une seule fois
        if (chestAnimation != null)
        {
            chestAnimation[openClipName].wrapMode = WrapMode.Once;
            chestAnimation.Play(openClipName);
            yield return new WaitForSeconds(
                chestAnimation[openClipName].length
            );
        }

        // Donner la clé
        if (localPlayer != null)
            localPlayer.GiveSpecificKey(keyId);

        // Faire apparaître le manuscript
        if (manuscript != null)
        {
            manuscript.SetActive(true);
            // Disparaît après 10 secondes
            yield return new WaitForSeconds(manuscriptDuration);
            manuscript.SetActive(false);
        }

        
    }

    // === BOUTON ANNULER ===
    public void PressCancel()
    {
        ClosePanel();
    }

    // === FONCTIONS INTERNES ===
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