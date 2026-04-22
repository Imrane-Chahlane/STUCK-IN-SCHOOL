using UnityEngine;
using TMPro;

public class BoardUI : MonoBehaviour
{
    public GameObject boardPanel;
    public TextMeshProUGUI boardText;
    public Transform player;
    public Transform board;
    public float closeDistance = 3f;

    [TextArea(5, 15)]
    public string message;

    private bool isOpen = false;

    void Update()
    {
        if (isOpen)
        {
            float dist = Vector3.Distance(player.position, board.position);
            Debug.Log("Distance: " + dist);
            if (dist > closeDistance)
                CloseBoard();
        }
    }

    public void ToggleBoard()
    {
        if (boardPanel == null) return;

        isOpen = !isOpen;
        boardPanel.SetActive(isOpen);

        if (isOpen)
        {
            if (boardText != null)
                boardText.text = message;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
            CloseBoard();
    }

    void CloseBoard()
    {
        isOpen = false;
        boardPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}