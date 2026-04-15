using UnityEngine;

public class TreasureOpening : MonoBehaviour // <--- Ce nom doit être identique au fichier
{
    public GameObject keyInChest; 

    public void Open()
    {
        Debug.Log("Le coffre s'ouvre !");
        if (keyInChest != null) keyInChest.SetActive(true);
    }
}