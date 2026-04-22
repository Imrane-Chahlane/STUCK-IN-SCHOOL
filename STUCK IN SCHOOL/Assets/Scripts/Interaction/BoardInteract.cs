using UnityEngine;

public class BoardInteract : Interactable
{
    public BoardUI boardUI;

    public override void Interact()
    {
        base.Interact(); // garde ton Debug.Log

        if (boardUI != null)
        {
            boardUI.ToggleBoard();
        }
    }
}
