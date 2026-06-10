using UnityEngine;
using TMPro;

public class CabinetKey : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    public float pickupRadius = 3f;
    public Camera playerCamera;

    [Header("Optional UI")]
    public TextMeshProUGUI pickupText;

    private bool collected = false;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        HideText();
    }

    private void Update()
    {
        if (collected) return;

        if (playerCamera == null)
            return;

        float distance = Vector3.Distance(playerCamera.transform.position, transform.position);

        if (distance <= pickupRadius)
        {
            ShowText();

            if (Input.GetKeyDown(KeyCode.E))
            {
                CollectKey();
            }
        }
        else
        {
            HideText();
        }
    }

    public void Interact()
    {
        CollectKey();
    }

    private void CollectKey()
    {
        if (collected) return;

        PlayerInventory inventory = FindOwnerInventory();

        if (inventory == null)
        {
            Debug.LogWarning("Owner PlayerInventory not found!");
            return;
        }

        inventory.GiveKey();

        collected = true;

        Debug.Log("Key collected from cabinet.");

        HideText();

        gameObject.SetActive(false);
    }

    private PlayerInventory FindOwnerInventory()
    {
        PlayerInventory[] inventories = FindObjectsByType<PlayerInventory>(FindObjectsSortMode.None);

        foreach (PlayerInventory inventory in inventories)
        {
            if (inventory.IsOwner)
                return inventory;
        }

        return null;
    }

    private void ShowText()
    {
        if (pickupText != null)
        {
            pickupText.gameObject.SetActive(true);
            pickupText.text = "Press E to take the key";
        }
    }

    private void HideText()
    {
        if (pickupText != null)
            pickupText.gameObject.SetActive(false);
    }
}