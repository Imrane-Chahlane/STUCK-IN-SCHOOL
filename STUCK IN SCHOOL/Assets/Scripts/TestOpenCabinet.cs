using UnityEngine;

public class TestOpenCabinet : MonoBehaviour
{
    public CabinetPuzzle cabinetPuzzle;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            cabinetPuzzle.OpenCabinet();
        }
    }
}