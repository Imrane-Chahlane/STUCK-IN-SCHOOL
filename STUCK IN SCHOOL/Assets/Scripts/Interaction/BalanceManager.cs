using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Cerveau du puzzle. Compare les poids des deux ascenseurs.
/// Quand les deux sont équilibrés ET à l'étage 0 → déclenche les portes.
/// Place ce script sur un GameObject vide "BalanceManager" dans la scène.
/// </summary>
public class BalanceManager : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // CONFIGURATION
    // ─────────────────────────────────────────────

    [Header("Les deux ascenseurs")]
    [Tooltip("Elevator1 — Player 1, démarre étage +1")]
    public ElevatorController elevatorA;

    [Tooltip("Elevator-1 — Player 2, démarre étage -1")]
    public ElevatorController elevatorB;

    [Header("Condition d'équilibre")]
    [Tooltip("Différence de poids maximale tolérée (kg) pour valider l'équilibre")]
    public float balanceTolerance = 2f;

    [Header("Condition de position")]
    [Tooltip("Les deux ascenseurs doivent-ils être à l'étage 0 pour valider ?")]
    public bool requireFloor0 = true;

    [Header("Événements Unity")]
    [Tooltip("Déclenché quand l'équilibre est atteint — connecte ta DoorController ici")]
    public UnityEvent onBalanceAchieved;

    [Tooltip("Déclenché quand l'équilibre est rompu après avoir été atteint")]
    public UnityEvent onBalanceLost;

    // ─────────────────────────────────────────────
    // ÉTAT INTERNE
    // ─────────────────────────────────────────────

    private bool _isBalanced = false;

    // ─────────────────────────────────────────────
    // INIT
    // ─────────────────────────────────────────────

    void Start()
    {
        if (elevatorA == null || elevatorB == null)
        {
            Debug.LogError("[BalanceManager] Assigne les deux ElevatorController dans l'Inspector !");
            return;
        }

        // Assigne les IDs
        elevatorA.elevatorID = "A";
        elevatorB.elevatorID = "B";

        Debug.Log("[BalanceManager] Prêt. Tolérance : " + balanceTolerance + " kg");
    }

    // ─────────────────────────────────────────────
    // APPELÉ PAR ElevatorController
    // ─────────────────────────────────────────────

    /// <summary>
    /// Appelé quand un ascenseur change de poids.
    /// </summary>
    public void OnWeightChanged(ElevatorController elevator, float newWeight)
    {
        Debug.Log($"[BalanceManager] Elevator {elevator.elevatorID} poids → {newWeight} kg");
        EvaluateBalance();
    }

    /// <summary>
    /// Appelé quand un ascenseur arrive à destination.
    /// </summary>
    public void OnElevatorArrived(ElevatorController elevator, int floor)
    {
        Debug.Log($"[BalanceManager] Elevator {elevator.elevatorID} arrivé étage {floor}");

        // Si arrivé à l'étage 0, ouvre les portes côté arrivée (optionnel)
        if (floor == 0)
            elevator.SetDoorsOpen(true);

        EvaluateBalance();
    }

    // ─────────────────────────────────────────────
    // LOGIQUE D'ÉQUILIBRE
    // ─────────────────────────────────────────────

    private void EvaluateBalance()
    {
        if (elevatorA == null || elevatorB == null) return;

        float weightA = elevatorA.GetTotalWeight();
        float weightB = elevatorB.GetTotalWeight();
        float diff    = Mathf.Abs(weightA - weightB);

        bool weightOK    = diff <= balanceTolerance;
        bool positionOK  = !requireFloor0 ||
                           (elevatorA.GetCurrentFloor() == 0 && elevatorB.GetCurrentFloor() == 0);

        bool balanced = weightOK && positionOK;

        Debug.Log($"[BalanceManager] A={weightA}kg B={weightB}kg diff={diff:F1}kg " +
                  $"| poids OK={weightOK} position OK={positionOK} → {(balanced ? "ÉQUILIBRÉ ✓" : "déséquilibré")}");

        if (balanced && !_isBalanced)
        {
            _isBalanced = true;
            Debug.Log("[BalanceManager] ★ ÉQUILIBRE ATTEINT → Portes s'ouvrent !");
            onBalanceAchieved?.Invoke();

            // Ouvre les portes des deux ascenseurs
            elevatorA.SetDoorsOpen(true);
            elevatorB.SetDoorsOpen(true);
        }
        else if (!balanced && _isBalanced)
        {
            _isBalanced = false;
            Debug.Log("[BalanceManager] Équilibre rompu → Portes se ferment.");
            onBalanceLost?.Invoke();

            elevatorA.SetDoorsOpen(false);
            elevatorB.SetDoorsOpen(false);
        }
    }

    // ─────────────────────────────────────────────
    // DEBUG VISUEL (Gizmos)
    // ─────────────────────────────────────────────

    void OnGUI()
    {
        if (!Application.isPlaying) return;

        float wA = elevatorA != null ? elevatorA.GetTotalWeight() : 0;
        float wB = elevatorB != null ? elevatorB.GetTotalWeight() : 0;

        GUI.color = _isBalanced ? Color.green : Color.white;
        GUI.Label(new Rect(10, 10, 300, 25), $"Elevator A : {wA} kg  |  Étage : {(elevatorA?.GetCurrentFloor())}");
        GUI.Label(new Rect(10, 35, 300, 25), $"Elevator B : {wB} kg  |  Étage : {(elevatorB?.GetCurrentFloor())}");
        GUI.Label(new Rect(10, 60, 300, 25), $"Différence : {Mathf.Abs(wA - wB):F1} kg  " +
                                              $"[tolérance : {balanceTolerance} kg]");
        GUI.Label(new Rect(10, 85, 300, 25), _isBalanced ? "★ ÉQUILIBRÉ" : "En attente d'équilibre...");
    }
}