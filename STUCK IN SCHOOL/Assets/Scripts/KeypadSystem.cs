using UnityEngine;

public class KeypadSystem : MonoBehaviour
{
    public string correctCode = "324";
    private string inputCode = "";
    
    // Référence à ton script de progression
    public GameProgress progressScript; 

    void Update()
    {
        // Entrées clavier (Alpha1 = touche 1 en haut du clavier)
        if (Input.GetKeyDown(KeyCode.Alpha1)) AddDigit("1");
        if (Input.GetKeyDown(KeyCode.Alpha2)) AddDigit("2");
        if (Input.GetKeyDown(KeyCode.Alpha3)) AddDigit("3");
        if (Input.GetKeyDown(KeyCode.Alpha4)) AddDigit("4");
        if (Input.GetKeyDown(KeyCode.Alpha5)) AddDigit("5");
        if (Input.GetKeyDown(KeyCode.Alpha6)) AddDigit("6");
        if (Input.GetKeyDown(KeyCode.Alpha7)) AddDigit("7");
        if (Input.GetKeyDown(KeyCode.Alpha8)) AddDigit("8");
        if (Input.GetKeyDown(KeyCode.Alpha9)) AddDigit("9");
        if (Input.GetKeyDown(KeyCode.Alpha0)) AddDigit("0");

        if (Input.GetKeyDown(KeyCode.Return)) CheckCode(); // Entrée
        if (Input.GetKeyDown(KeyCode.R)) ResetCode();      // R pour effacer
    }

    void AddDigit(string digit)
    {
        if (inputCode.Length < 3)
        {
            inputCode += digit;
            Debug.Log("Code en cours : " + inputCode);
        }
    }

    void CheckCode()
    {
        if (inputCode == correctCode)
        {
            Debug.Log("✅ Code correct !");
            
            // 1. On prévient le GameManager (logique)
            if (progressScript != null) progressScript.ReceiveKey();

            // 2. On dit au coffre de s'ouvrir (visuel)
            TreasureOpening chest = GetComponent<TreasureOpening>();
            if (chest != null) 
            {
                chest.Open();
            }
        }
        else
        {
            Debug.Log("❌ Code incorrect !");
            ResetCode();
        }
    }

    void ResetCode() => inputCode = "";
}