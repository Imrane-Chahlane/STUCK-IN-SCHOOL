using System.Collections;
using UnityEngine;

namespace NavKeypad
{
    public class KeypadDoorController : MonoBehaviour
    {
        [SerializeField] private string[] doorPaths = new string[]
        {
            "Lab_Player1/Door4/Door4.2",
            "Lab_Player2/Door4/Door4.2"
        };

        [SerializeField] private Vector3 slideDirection = Vector3.forward;
        [SerializeField] private float slideDistance = 0.8f;
        [SerializeField] private float slideSpeed = 2f;

        private void Start()
        {
            Keypad keypad = FindObjectOfType<Keypad>();
            if (keypad == null)
            {
                Debug.LogError("[KeypadDoorController] No Keypad found in scene!");
                return;
            }
            keypad.OnAccessGranted.AddListener(OpenAllDoors);
        }

        private void OpenAllDoors()
        {
            foreach (string path in doorPaths)
            {
                GameObject doorObj = GameObject.Find(path);
                if (doorObj == null)
                {
                    Debug.LogWarning($"[KeypadDoorController] Door not found: {path}");
                    continue;
                }
                DoorSlider slider = doorObj.GetComponent<DoorSlider>();
                if (slider == null)
                {
                    slider = doorObj.AddComponent<DoorSlider>();
                    slider.Configure(slideDirection, slideDistance, slideSpeed);
                }
                slider.Open();
            }
        }
    }
}
