using UnityEngine;

public class DiscRotator : MonoBehaviour
{
    public float rotationSpeed = 200f;

    [Header("Rotation Axis")]
    public Vector3 rotationAxis = Vector3.up;

    private bool isRotating = false;

    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    public void StartRotation()
    {
        isRotating = true;
    }

    public void StopRotation()
    {
        isRotating = false;
    }
}