using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public bool hasKey = false;

    // Cette fonction est appelée par le clavier quand le code est bon
    public void ReceiveKey()
    {
        hasKey = true;
        Debug.Log("Système : Clé enregistrée dans le GameManager !");
    }
}
