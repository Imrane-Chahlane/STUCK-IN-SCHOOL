using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool hasKey = false;

    public void GiveKey()
    {
        hasKey = true;

        Debug.Log("KEY RECEIVED");

        // SAFE GLOBAL UI CALL
        if (KeyUIManager.Instance != null)
            KeyUIManager.Instance.ShowKeyCollected();
    }
}