using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public GameProgress progress; 

    // Cette fonction devra être appelée par la Personne 1 (Interaction E)
    public void TryOpenDoor()
    {
        if (progress != null && progress.hasKey)
        {
            Debug.Log("Porte ouverte ! Fin du niveau.");
            gameObject.SetActive(false); // La porte disparaît
        }
        else
        {
            Debug.Log("La porte est fermée. Trouvez le code sur le tableau.");
        }
    }
}