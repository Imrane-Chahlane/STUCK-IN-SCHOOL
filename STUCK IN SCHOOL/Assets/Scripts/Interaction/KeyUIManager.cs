using UnityEngine;
using System.Collections;

public class KeyUIManager : MonoBehaviour
{
    public static KeyUIManager Instance;
    public GameObject keyTextUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
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