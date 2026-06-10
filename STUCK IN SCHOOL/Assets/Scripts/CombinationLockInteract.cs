using UnityEngine;

public class CombinationLockInteract : MonoBehaviour, IInteractable
{
    public enum WheelAxis
    {
        X,
        Y,
        Z
    }

    [Header("Code Settings")]
    public string correctCode = "32485";
    public int maxCodeLength = 5;

    [Header("References")]
    public CabinetPuzzle cabinetPuzzle;

    [Header("Cameras")]
    public Camera gameplayCamera;
    public Camera lockZoomCamera;

    [Header("Interaction")]
    public float interactionRadius = 4f;

    [Header("Lock Wheels")]
    public Transform[] digitWheels;
    public WheelAxis wheelRotationAxis = WheelAxis.Z;
    public float anglePerDigit = 36f;
    public float rotationSign = 1f;
    public float digitAngleOffset = 0f;

    private string currentCode = "";
    private bool isTyping = false;
    private bool solved = false;

    private Vector3[] originalWheelRotations;

    private void Start()
    {
        if (gameplayCamera == null)
            gameplayCamera = Camera.main;

        if (lockZoomCamera != null)
            lockZoomCamera.gameObject.SetActive(false);

        if (gameplayCamera != null)
            gameplayCamera.gameObject.SetActive(true);

        SaveOriginalWheelRotations();
    }

    private void Update()
    {
        if (solved) return;

        if (!isTyping && Input.GetKeyDown(KeyCode.E))
        {
            TryStartTypingByDistance();
        }

        if (!isTyping) return;

        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) AddDigit("0");
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) AddDigit("1");
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) AddDigit("2");
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) AddDigit("3");
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) AddDigit("4");
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) AddDigit("5");
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) AddDigit("6");
        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) AddDigit("7");
        if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) AddDigit("8");
        if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) AddDigit("9");

        if (Input.GetKeyDown(KeyCode.Backspace))
            DeleteDigit();

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            SubmitCode();

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelTyping();
    }

    public void Interact()
    {
        StartTyping();
    }

    private void TryStartTypingByDistance()
    {
        Camera cam = gameplayCamera != null ? gameplayCamera : Camera.main;

        if (cam == null)
        {
            Debug.LogWarning("Gameplay camera is missing.");
            return;
        }

        float distance = Vector3.Distance(cam.transform.position, transform.position);

        if (distance <= interactionRadius)
        {
            StartTyping();
        }
        else
        {
            Debug.Log("You are too far from the lock. Distance: " + distance);
        }
    }

    private void StartTyping()
    {
        if (solved) return;

        isTyping = true;
        currentCode = "";

        ResetWheelsToOriginal();
        SwitchToLockCamera();

        Debug.Log("CADENAS DETECTED - Tape le code avec le clavier puis appuie sur Enter.");
        Debug.Log("Code actuel : " + FormatCodeForDisplay());
    }

    private void AddDigit(string digit)
    {
        if (currentCode.Length >= maxCodeLength) return;

        currentCode += digit;

        int wheelIndex = currentCode.Length - 1;
        int digitValue = int.Parse(digit);

        RotateWheel(wheelIndex, digitValue);

        Debug.Log("Code actuel : " + FormatCodeForDisplay());
    }

    private void DeleteDigit()
    {
        if (currentCode.Length <= 0) return;

        int wheelIndex = currentCode.Length - 1;

        currentCode = currentCode.Substring(0, currentCode.Length - 1);

        ResetWheel(wheelIndex);

        Debug.Log("Code actuel : " + FormatCodeForDisplay());
    }

    private void SubmitCode()
        {
            Debug.Log("Code entré : " + currentCode);

            if (currentCode == correctCode)
            {
                solved = true;
                isTyping = false;

                Debug.Log("Code correct ! Ouverture du casier.");

                SwitchToGameplayCamera();

                if (cabinetPuzzle != null)
                    cabinetPuzzle.OpenCabinet();
                else
                    Debug.LogWarning("CabinetPuzzle n'est pas assigné !");

                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Code incorrect ! Retour à la caméra normale.");

                currentCode = "";
                isTyping = false;

                ResetWheelsToOriginal();

                SwitchToGameplayCamera();

                Debug.Log("Appuie sur E pour réessayer.");
            }
        }

    private void CancelTyping()
    {
        isTyping = false;
        currentCode = "";

        ResetWheelsToOriginal();
        SwitchToGameplayCamera();

        Debug.Log("Saisie du code annulée.");
    }

    private void SwitchToLockCamera()
    {
        if (gameplayCamera != null)
            gameplayCamera.gameObject.SetActive(false);

        if (lockZoomCamera != null)
            lockZoomCamera.gameObject.SetActive(true);
    }

    private void SwitchToGameplayCamera()
    {
        if (lockZoomCamera != null)
            lockZoomCamera.gameObject.SetActive(false);

        if (gameplayCamera != null)
            gameplayCamera.gameObject.SetActive(true);
    }

    private void SaveOriginalWheelRotations()
    {
        if (digitWheels == null) return;

        originalWheelRotations = new Vector3[digitWheels.Length];

        for (int i = 0; i < digitWheels.Length; i++)
        {
            if (digitWheels[i] != null)
                originalWheelRotations[i] = digitWheels[i].localEulerAngles;
        }
    }

    private void RotateWheel(int wheelIndex, int digitValue)
    {
        if (digitWheels == null) return;
        if (originalWheelRotations == null) return;
        if (wheelIndex < 0 || wheelIndex >= digitWheels.Length) return;
        if (digitWheels[wheelIndex] == null) return;

        Vector3 euler = originalWheelRotations[wheelIndex];
        float angle = digitAngleOffset + digitValue * anglePerDigit * rotationSign;

        if (wheelRotationAxis == WheelAxis.X)
            euler.x = angle;
        else if (wheelRotationAxis == WheelAxis.Y)
            euler.y = angle;
        else if (wheelRotationAxis == WheelAxis.Z)
            euler.z = angle;

        digitWheels[wheelIndex].localEulerAngles = euler;
    }

    private void ResetWheel(int wheelIndex)
    {
        if (digitWheels == null) return;
        if (originalWheelRotations == null) return;
        if (wheelIndex < 0 || wheelIndex >= digitWheels.Length) return;
        if (digitWheels[wheelIndex] == null) return;

        digitWheels[wheelIndex].localEulerAngles = originalWheelRotations[wheelIndex];
    }

    private void ResetWheelsToOriginal()
    {
        if (digitWheels == null || originalWheelRotations == null) return;

        for (int i = 0; i < digitWheels.Length; i++)
        {
            if (digitWheels[i] != null)
                digitWheels[i].localEulerAngles = originalWheelRotations[i];
        }
    }

    private string FormatCodeForDisplay()
    {
        string display = currentCode;

        while (display.Length < maxCodeLength)
            display += "_";

        return display;
    }
}