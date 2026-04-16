using UnityEngine;

public class AddCollidersToChildren : MonoBehaviour
{
    void Start()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<Collider>() == null)
            {
                child.gameObject.AddComponent<BoxCollider>();
            }
        }
    }
}