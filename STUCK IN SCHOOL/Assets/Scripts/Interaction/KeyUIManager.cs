using UnityEngine;
using TMPro;
using System.Collections;

public class KeyUIManager : MonoBehaviour
{
    public static KeyUIManager Instance;

    public GameObject keyTextUI;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowKeyCollected()
    {
        StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        keyTextUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        keyTextUI.SetActive(false);
    }
}