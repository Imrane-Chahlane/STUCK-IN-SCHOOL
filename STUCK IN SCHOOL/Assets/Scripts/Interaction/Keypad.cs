using UnityEngine;
using TMPro;
using System.Collections;

public class Keypad : MonoBehaviour
{
    public string correctCode = "1234";

    private string input = "";

    public TMP_Text display;

    // ──────────────────────────────────────────
    // AJOUTE CES 2 LIGNES (glisser dans Inspector)
    public Animator chestAnimator;  // glisser Treasure ici
    public KeyDoor keyDoor;         // glisser Door ici
    public KeyDoor keyDoor2;        // glisser keyDoor2 ici
    // ──────────────────────────────────────────

    private PlayerInventory inventory;

    public void SetInventory(PlayerInventory inv)
    {
        inventory = inv;
    }

    public void AddNumber(string number)
    {
        if (input.Length >= 4) return; // max 4 chiffres
        input += number;
        display.text = input;
    }

    public void Delete()
    {
        if (input.Length > 0)
        {
            input = input.Substring(0, input.Length - 1);
            display.text = input;
        }
    }

    public void Submit()
    {
        if (inventory == null)
            inventory = FindFirstObjectByType<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogError("NO PLAYER INVENTORY FOUND!");
            return;
        }

        if (input == correctCode)
        {
            Debug.Log("Correct Code!");

            // Feedback vert
            display.color = Color.green;
            display.text = "CORRECT !";

            // Donne la clé
            inventory.GiveKey();

            // Ouvre le coffre
            if (chestAnimator != null)
                chestAnimator.SetBool("Open", true);

            // Ouvre la porte après 2 secondes
            StartCoroutine(OpenDoorAfterDelay(2f));

            input = "";
        }
        else
        {
            Debug.Log("Wrong Code!");

            // Feedback rouge
            display.color = Color.red;
            display.text = "ERREUR !";

            // Remet à zéro après 1 seconde
            StartCoroutine(ResetAfterDelay(1f));

            input = "";
        }
    }

    private IEnumerator OpenDoorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (keyDoor != null)
            keyDoor.ForceOpen();
        if (keyDoor2 != null)      // ← AJOUTE CES 2 LIGNES
            keyDoor2.ForceOpen();
    }

    private IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        display.color = Color.white;
        display.text = "";
    }
}