using UnityEngine;

public class DoorF1Simple : MonoBehaviour
{
    public GameObject doorPart; // Assigne Front Door_001_Doors_0 ici

    private bool isOpen = false;
    private MeshCollider meshCollider;

    void Start()
    {
        meshCollider = doorPart.GetComponent<MeshCollider>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            isOpen = !isOpen;

            if (isOpen)
                doorPart.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
            else
                doorPart.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

            if (meshCollider != null)
                meshCollider.enabled = !isOpen;
        }
    }
}