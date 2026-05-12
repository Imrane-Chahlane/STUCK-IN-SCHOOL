using Unity.Netcode;
using UnityEngine;
using TMPro;

public class Keypad : NetworkBehaviour, IInteractable
{
    public string correctCode = "1234";
    private string input = "";
    public TMP_Text display;
    private PlayerInventory inventory;

    public void Interact() { }

    public void SetInventory(PlayerInventory inv)
    {
        inventory = inv;
    }

    public void AddNumber(string number)
    {
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
        {
            foreach (var inv in FindObjectsByType<PlayerInventory>(FindObjectsSortMode.None))
            {
                if (inv.IsOwner)
                {
                    inventory = inv;
                    break;
                }
            }
        }

        if (inventory == null)
        {
            Debug.LogError("NO LOCAL PLAYER INVENTORY FOUND!");
            return;
        }

        if (input == correctCode)
        {
            inventory.GiveKey();
            input = "";
            display.text = "";
        }
        else
        {
            Debug.Log("Wrong Code!");
            input = "";
            display.text = "";
        }
    }
}