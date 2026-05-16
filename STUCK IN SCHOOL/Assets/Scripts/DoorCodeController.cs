using UnityEngine;
using System.Collections;

public class DoorCodeController : MonoBehaviour
{
    [Header("Code")]
    public string correctCode = "54321";

    [Header("Door")]
    public Transform doorTransform;
    public float openAngle = 90f;
    public float animationSpeed = 2f;

    [Header("Collider")]
    public Collider doorCollider;

    private bool isOpen = false;
    private bool isAnimating = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = doorTransform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    // Appelé depuis le KeypadManager existant
    public void TryCode(string enteredCode)
    {
        if (enteredCode == correctCode && !isOpen)
        {
            StartCoroutine(OpenDoor());
        }
        else
        {
            Debug.Log("Code incorrect !");
        }
    }

    IEnumerator OpenDoor()
    {
        isAnimating = true;

        // Désactiver le collider pour traverser
        if (doorCollider != null)
            doorCollider.enabled = false;

        // Animation ouverture
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * animationSpeed;
            doorTransform.rotation = Quaternion.Lerp(
                closedRotation, openRotation, t
            );
            yield return null;
        }

        isOpen = true;
        isAnimating = false;
    }

    public void CloseDoor()
    {
        if (isOpen)
            StartCoroutine(CloseDoorRoutine());
    }

    IEnumerator CloseDoorRoutine()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * animationSpeed;
            doorTransform.rotation = Quaternion.Lerp(
                openRotation, closedRotation, t
            );
            yield return null;
        }

        // Réactiver le collider
        if (doorCollider != null)
            doorCollider.enabled = true;

        isOpen = false;
    }
}