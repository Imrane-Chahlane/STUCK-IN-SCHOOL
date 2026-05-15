using UnityEngine;
using Unity.Netcode;

public class PlayerDiscInteraction : NetworkBehaviour
{
    [Header("Interaction")]
    public float interactionDistance = 3f;
    public Camera playerCamera;

    [Header("Holding Disc")]
    public Transform holdPoint;

    private GameObject heldDisc;

    public override void OnNetworkSpawn()
    {
        // Important en multijoueur :
        // chaque joueur contrôle seulement son propre Player.
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        SetupCameraAndHoldPoint();
    }

    void SetupCameraAndHoldPoint()
    {
        // Cherche la vraie caméra du joueur
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }

        if (playerCamera == null)
        {
            Debug.LogError("No camera found for PlayerDiscInteraction.");
            return;
        }

        // Crée automatiquement HoldPoint devant la caméra
        if (holdPoint == null)
        {
            GameObject hp = new GameObject("HoldPoint");
            hp.transform.SetParent(playerCamera.transform);
            hp.transform.localPosition = new Vector3(0.4f, -0.3f, 1f);
            hp.transform.localRotation = Quaternion.identity;
            holdPoint = hp.transform;
        }
    }

    void Update()
    {
        if (playerCamera == null || holdPoint == null)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    void TryInteract()
{
    Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

    Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionDistance, Color.red, 2f);

    RaycastHit[] hits = Physics.RaycastAll(ray, interactionDistance);

    if (hits.Length == 0)
    {
        if (heldDisc != null)
        {
            DropDisc();
        }
        else
        {
            Debug.Log("Nothing detected by Raycast");
        }

        return;
    }

    System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

    foreach (RaycastHit hit in hits)
    {
        Debug.Log("Hit object: " + hit.collider.name);

        // Ignorer le player
        if (hit.collider.GetComponentInParent<PlayerDiscInteraction>() != null)
        {
            Debug.Log("Ignored player object: " + hit.collider.name);
            continue;
        }

        // Si je porte un disque et je regarde le projecteur
        ProjectorSystem projector = hit.collider.GetComponentInParent<ProjectorSystem>();

        if (projector != null && heldDisc != null)
        {
            Debug.Log("Projector detected");
            projector.InsertDisc(heldDisc);
            heldDisc = null;
            return;
        }

        // Si je ne porte rien et je regarde un disque
        FilmDisc disc = hit.collider.GetComponentInParent<FilmDisc>();

        if (disc != null && heldDisc == null)
        {
            Debug.Log("FilmDisc detected: " + disc.name);
            PickUpDisc(disc.gameObject);
            return;
        }
    }

    // Si on arrive ici, ça veut dire :
    // on a appuyé sur E, mais ce qu'on regarde n'est ni disque ni projecteur.
    // Donc si on porte un disque, on le droppe.
    if (heldDisc != null)
    {
        DropDisc();
    }
    else
    {
        Debug.Log("Object hit, but no FilmDisc or ProjectorSystem found.");
    }
}

    void PickUpDisc(GameObject discObject)
    {
        heldDisc = discObject;

        Rigidbody rb = heldDisc.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = heldDisc.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        heldDisc.transform.SetParent(holdPoint);
        heldDisc.transform.localPosition = Vector3.zero;
        heldDisc.transform.localRotation = Quaternion.identity;

        Debug.Log("Picked up disc: " + heldDisc.name);
    }
    void DropDisc()
{
    if (heldDisc == null)
        return;

    GameObject discToDrop = heldDisc;
    heldDisc = null;

    // Détacher de la main
    discToDrop.transform.SetParent(null);

    // Placer le disque devant le joueur
    Vector3 dropPosition = playerCamera.transform.position + playerCamera.transform.forward * 1.2f;
    dropPosition.y -= 0.5f;

    discToDrop.transform.position = dropPosition;
    discToDrop.transform.rotation = Quaternion.identity;

    // Réactiver le collider
    Collider col = discToDrop.GetComponent<Collider>();
    if (col != null)
    {
        col.enabled = true;
    }

    // Réactiver la physique
    Rigidbody rb = discToDrop.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    Debug.Log("Dropped disc: " + discToDrop.name);
}
}