using System.Collections;
using UnityEngine;

/// <summary>
/// Contrôle un ascenseur entre 3 étages (-1, 0, +1).
/// Fonctionne via Lerp fluide — pas besoin d'Animator Controller.
/// À attacher sur Elevator1 et Elevator-1.
/// </summary>
public class ElevatorController : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // CONFIGURATION — à remplir dans l'Inspector
    // ─────────────────────────────────────────────

    [Header("Étages — assigne les 3 transforms dans l'Inspector")]
    [Tooltip("Transform de l'étage -1 (position basse)")]
    public Transform floorMinus1;

    [Tooltip("Transform de l'étage 0 (objectif)")]
    public Transform floor0;

    [Tooltip("Transform de l'étage +1 (position haute)")]
    public Transform floor1;

    [Header("Mouvement")]
    [Tooltip("Vitesse de déplacement (unités Unity par seconde)")]
    public float moveSpeed = 80f;

    [Tooltip("Tolérance pour considérer l'ascenseur 'arrivé' à destination")]
    public float arrivalThreshold = 5f;

    [Header("Plateau de détection du poids")]
    [Tooltip("Le trigger collider sur la plateforme de l'ascenseur")]
    public Collider weightPlatform;

    [Header("Portes (enfants du modèle Sketchfab)")]
    [Tooltip("Transform de la porte gauche — assigne depuis la hiérarchie du lift_model")]
    public Transform doorLeft;

    [Tooltip("Transform de la porte droite")]
    public Transform doorRight;

    [Tooltip("Distance d'ouverture des portes (en unités locales du modèle)")]
    public float doorOpenDistance = 60f;

    [Tooltip("Vitesse d'ouverture/fermeture des portes")]
    public float doorSpeed = 40f;

    // ─────────────────────────────────────────────
    // ÉTAT INTERNE
    // ─────────────────────────────────────────────

    // Étage actuel : -1, 0, ou 1
    private int _currentFloor;

    // Position Y cible pour le Lerp
    private Vector3 _targetPosition;

    // Poids total des objets sur la plateforme
    private float _totalWeight = 0f;

    // L'ascenseur est-il en train de bouger ?
    private bool _isMoving = false;

    // Portes ouvertes ou fermées
    private bool _doorsOpen = false;

    // Positions locales initiales des portes (pour fermeture)
    private Vector3 _doorLeftClosedPos;
    private Vector3 _doorRightClosedPos;

    // Référence au BalanceManager (se trouve automatiquement)
    private BalanceManager _balanceManager;

    // ID de cet ascenseur (A ou B) — assigné par le BalanceManager
    [HideInInspector]
    public string elevatorID = "A";

    // ─────────────────────────────────────────────
    // DÉMARRAGE
    // ─────────────────────────────────────────────

    void Start()
    {
        // Mémorise les positions fermées des portes
        if (doorLeft != null)  _doorLeftClosedPos  = doorLeft.localPosition;
        if (doorRight != null) _doorRightClosedPos = doorRight.localPosition;

        // Détermine l'étage de départ selon elevatorID
        // Elevator A (Player 1) commence à l'étage +1
        // Elevator B (Player 2) commence à l'étage -1
        if (elevatorID == "A")
        {
            _currentFloor = 1;
            _targetPosition = floor1 != null ? floor1.position : transform.position;
        }
        else
        {
            _currentFloor = -1;
            _targetPosition = floorMinus1 != null ? floorMinus1.position : transform.position;
        }

        // Snap immédiat à la position de départ
        transform.position = _targetPosition;

        // Trouve le BalanceManager dans la scène
        _balanceManager = FindObjectOfType<BalanceManager>();
        if (_balanceManager == null)
            Debug.LogWarning($"[ElevatorController {elevatorID}] BalanceManager introuvable dans la scène !");
    }

    // ─────────────────────────────────────────────
    // UPDATE — MOUVEMENT LERP
    // ─────────────────────────────────────────────

    void Update()
    {
        // Lerp vers la position cible
        if (Vector3.Distance(transform.position, _targetPosition) > arrivalThreshold)
        {
            _isMoving = true;
            transform.position = Vector3.MoveTowards(
                transform.position,
                _targetPosition,
                moveSpeed * Time.deltaTime
            );
        }
        else if (_isMoving)
        {
            // L'ascenseur vient d'arriver
            _isMoving = false;
            transform.position = _targetPosition;
            OnArrived();
        }

        // Animation des portes
        AnimateDoors();
    }

    // ─────────────────────────────────────────────
    // ARRIVÉE À UN ÉTAGE
    // ─────────────────────────────────────────────

    private void OnArrived()
    {
        Debug.Log($"[Elevator {elevatorID}] Arrivé à l'étage {_currentFloor}");

        // Notifie le BalanceManager que cet ascenseur a bougé
        _balanceManager?.OnElevatorArrived(this, _currentFloor);
    }

    // ─────────────────────────────────────────────
    // DÉPLACEMENT — appelé par BalanceManager
    // ─────────────────────────────────────────────

    /// <summary>
    /// Demande à l'ascenseur de se déplacer vers l'étage cible.
    /// floor : -1, 0, ou 1
    /// </summary>
    public void MoveToFloor(int floor)
    {
        floor = Mathf.Clamp(floor, -1, 1);

        // Ne bouge pas si déjà à cet étage
        if (floor == _currentFloor && !_isMoving) return;

        _currentFloor = floor;

        // Ferme les portes avant de bouger
        SetDoorsOpen(false);

        // Détermine la position Y cible
        switch (floor)
        {
            case  1: _targetPosition = floor1     != null ? floor1.position     : transform.position; break;
            case  0: _targetPosition = floor0     != null ? floor0.position     : transform.position; break;
            case -1: _targetPosition = floorMinus1 != null ? floorMinus1.position : transform.position; break;
        }

        Debug.Log($"[Elevator {elevatorID}] Départ vers étage {floor} → {_targetPosition}");
    }

    // ─────────────────────────────────────────────
    // PORTES
    // ─────────────────────────────────────────────

    public void SetDoorsOpen(bool open)
    {
        _doorsOpen = open;
    }

    private void AnimateDoors()
    {
        if (doorLeft == null || doorRight == null) return;

        if (_doorsOpen)
        {
            // Ouvre : déplace les portes vers l'extérieur sur l'axe X local
            doorLeft.localPosition = Vector3.MoveTowards(
                doorLeft.localPosition,
                _doorLeftClosedPos  + Vector3.left  * doorOpenDistance,
                doorSpeed * Time.deltaTime
            );
            doorRight.localPosition = Vector3.MoveTowards(
                doorRight.localPosition,
                _doorRightClosedPos + Vector3.right * doorOpenDistance,
                doorSpeed * Time.deltaTime
            );
        }
        else
        {
            // Ferme : revient aux positions initiales
            doorLeft.localPosition  = Vector3.MoveTowards(doorLeft.localPosition,  _doorLeftClosedPos,  doorSpeed * Time.deltaTime);
            doorRight.localPosition = Vector3.MoveTowards(doorRight.localPosition, _doorRightClosedPos, doorSpeed * Time.deltaTime);
        }
    }

    // ─────────────────────────────────────────────
    // DÉTECTION DU POIDS (OnTrigger)
    // ─────────────────────────────────────────────

    private void OnTriggerEnter(Collider other)
    {
        WeightObject weight = other.GetComponent<WeightObject>();
        if (weight != null)
        {
            _totalWeight += weight.weightValue;
            Debug.Log($"[Elevator {elevatorID}] +{weight.weightValue}kg → Total : {_totalWeight}kg");
            _balanceManager?.OnWeightChanged(this, _totalWeight);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        WeightObject weight = other.GetComponent<WeightObject>();
        if (weight != null)
        {
            _totalWeight = Mathf.Max(0, _totalWeight - weight.weightValue);
            Debug.Log($"[Elevator {elevatorID}] -{weight.weightValue}kg → Total : {_totalWeight}kg");
            _balanceManager?.OnWeightChanged(this, _totalWeight);
        }
    }

    // ─────────────────────────────────────────────
    // ACCESSEURS
    // ─────────────────────────────────────────────

    public float GetTotalWeight() => _totalWeight;
    public int   GetCurrentFloor() => _currentFloor;
    public bool  IsMoving() => _isMoving;
}