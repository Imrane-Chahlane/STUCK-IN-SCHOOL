using UnityEngine;
using TMPro;

public class Keypad : MonoBehaviour
{
    public string correctCode = "1234";

    private string input = "";

    public TMP_Text display;

    private PlayerInventory inventory;


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
        // Find player inventory dynamically
        if (inventory == null)
        {
            inventory = FindFirstObjectByType<PlayerInventory>();
        }
        Debug.Log("Submit called");
        Debug.Log("Inventory = " + inventory);

        if (inventory == null)
        {
            Debug.LogError("NO PLAYER INVENTORY FOUND!");
            return;
        }

        if (input == correctCode)
        {
            Debug.Log("Correct Code!");

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