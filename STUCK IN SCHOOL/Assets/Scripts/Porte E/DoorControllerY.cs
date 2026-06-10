using UnityEngine;
using System.Collections;

public class DoorControllerY : MonoBehaviour
{
    public Transform doorTransform;
    public Collider doorCollider;
    public float animationSpeed = 2f;

    private bool isOpen = false;
    private bool isAnimating = false;

    public void Open()
    {
        if (!isOpen && !isAnimating)
            StartCoroutine(OpenSequence());
    }

    public void Close()
    {
        if (isOpen && !isAnimating)
            StartCoroutine(CloseSequence());
    }

    IEnumerator OpenSequence()
    {
        isAnimating = true;
        if (doorCollider != null)
            doorCollider.enabled = false;

        float t = 0f;
        Vector3 startRot = doorTransform.localEulerAngles;
        Vector3 endRot = new Vector3(0f, 90f, 0f);

        while (t < 1f)
        {
            t += Time.deltaTime * animationSpeed;
            float smooth = t * t * (3f - 2f * t);
            doorTransform.localEulerAngles = Vector3.Lerp(
                startRot, endRot, smooth);
            yield return null;
        }

        isOpen = true;
        isAnimating = false;
    }

    IEnumerator CloseSequence()
    {
        isAnimating = true;

        float t = 0f;
        Vector3 startRot = doorTransform.localEulerAngles;
        Vector3 endRot = new Vector3(0f, 0f, 0f);

        while (t < 1f)
        {
            t += Time.deltaTime * animationSpeed;
            float smooth = t * t * (3f - 2f * t);
            doorTransform.localEulerAngles = Vector3.Lerp(
                startRot, endRot, smooth);
            yield return null;
        }

        if (doorCollider != null)
            doorCollider.enabled = true;

        isOpen = false;
        isAnimating = false;
    }
}
