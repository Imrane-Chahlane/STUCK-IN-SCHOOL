using UnityEngine;

public class CabinetPuzzle : MonoBehaviour
{
    [Header("Cabinet Animation")]
    public Animator cabinetAnimator;
    public string openAnimationName = "Cabinet_Open";

    [Header("Objects")]
    public GameObject combinationLock;
    public GameObject keyInside;

    private bool isOpened = false;

    private void Start()
    {
        if (keyInside != null)
            keyInside.SetActive(false);

        if (cabinetAnimator != null)
            cabinetAnimator.enabled = false;
    }

    public void OpenCabinet()
    {
        if (isOpened) return;

        isOpened = true;

        Debug.Log("Correct code! Cabinet is opening.");

        if (combinationLock != null)
            combinationLock.SetActive(false);

        if (cabinetAnimator != null)
        {
            cabinetAnimator.enabled = true;
            cabinetAnimator.Play(openAnimationName, 0, 0f);
        }

        Invoke(nameof(ShowKey), 1f);
    }

    private void ShowKey()
    {
        if (keyInside != null)
            keyInside.SetActive(true);
    }
}