using UnityEngine;

/// <summary>
/// À attacher sur tout objet que les joueurs peuvent poser dans l'ascenseur.
/// Donne une valeur de poids détectée par ElevatorController via OnTrigger.
/// </summary>
public class WeightObject : MonoBehaviour
{
    [Header("Poids de cet objet")]
    [Tooltip("Valeur en kg — utilisée par BalanceManager pour comparer les deux ascenseurs")]
    public float weightValue = 10f;

    [Header("Visuel optionnel")]
    [Tooltip("Affiche le poids en kg au-dessus de l'objet dans la scène (debug)")]
    public bool showWeightLabel = true;

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (showWeightLabel)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 20f, $"{weightValue} kg");
        }
#endif
    }
}