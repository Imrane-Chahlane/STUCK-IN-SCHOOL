using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NavKeypad
{
    [RequireComponent(typeof(Camera))]
    public class KeypadBootstrapper : MonoBehaviour
    {
        [Header("Door Paths")]
        [SerializeField] private string[] doorPaths = new string[]
        {
            "Lab_Player1/Door4/Door4.2",
            "Lab_Player2/Door4/Door4.2"
        };

        [Header("Door Slide")]
        [SerializeField] private Vector3 slideDirection = Vector3.back;
        [SerializeField] private float slideDistance = 0.8f;
        [SerializeField] private float slideSpeed = 2f;

        [Header("Interaction")]
        [SerializeField] private float maxDistance = 5f;

        private Camera cam;
        private Keypad keypad;
        private readonly List<DoorSlider> doors = new List<DoorSlider>();
        private bool setupComplete = false;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            gameObject.tag = "MainCamera";
        }

        private void Start()
        {
            keypad = FindObjectOfType<Keypad>();
            if (keypad == null)
            {
                Debug.LogError("[KeypadBootstrapper] No Keypad found! Drag Assets/Keypad/Prefabs/Keypad.prefab into the scene.");
                return;
            }
            keypad.OnAccessGranted.AddListener(OpenAllDoors);
            Debug.Log("[KeypadBootstrapper] Keypad found and wired.");

            FindDoors();

            setupComplete = true;
        }

        private void FindDoors()
        {
            doors.Clear();
            foreach (string path in doorPaths)
            {
                GameObject go = ResolvePath(path);
                if (go == null)
                {
                    Debug.LogWarning($"[KeypadBootstrapper] Door not found: {path}");
                    continue;
                }
                DoorSlider slider = go.GetComponent<DoorSlider>();
                if (slider == null)
                {
                    slider = go.AddComponent<DoorSlider>();
                    slider.Configure(slideDirection, slideDistance, slideSpeed);
                }
                doors.Add(slider);
                Debug.Log($"[KeypadBootstrapper] Door ready: {path}");
            }
        }

        private GameObject ResolvePath(string path)
        {
            int slash = path.IndexOf('/');
            if (slash < 0) return GameObject.Find(path);

            string rootName = path.Substring(0, slash);
            string rest = path.Substring(slash + 1);

            GameObject root = GameObject.Find(rootName);
            if (root != null)
            {
                Transform t = root.transform.Find(rest);
                if (t != null) return t.gameObject;
            }

            foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (!go.scene.IsValid()) continue;
                if (go.name == rootName)
                {
                    Transform t = go.transform.Find(rest);
                    if (t != null) return t.gameObject;
                }
            }
            return null;
        }

        private void Update()
        {
            if (!setupComplete) return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Ray ray = cam.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0f));
                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                {
                    KeypadButton btn = hit.collider.GetComponent<KeypadButton>();
                    if (btn != null)
                    {
                        Debug.Log($"[KeypadBootstrapper] Button pressed: {btn.name}");
                        btn.PressButton();
                    }
                }
            }
        }

        private void OpenAllDoors()
        {
            Debug.Log("[KeypadBootstrapper] Correct code! Opening doors...");
            foreach (DoorSlider door in doors)
            {
                if (door != null) door.Open();
            }
        }
    }
}
