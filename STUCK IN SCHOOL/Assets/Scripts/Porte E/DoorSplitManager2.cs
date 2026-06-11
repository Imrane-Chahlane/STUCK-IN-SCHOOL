using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Collections;

public class DoorSplitManager2 : NetworkBehaviour
{
    [Header("Doors")]
    public DoorController door1;
    public DoorController door2;

    [Header("UI")]
    public TextMeshProUGUI warningText;

    // Qui est devant quelle porte
    // 0 = personne, ClientId du joueur
    private NetworkVariable<ulong> _playerAtDoor1 = new NetworkVariable<ulong>(
        ulong.MaxValue,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private NetworkVariable<ulong> _playerAtDoor2 = new NetworkVariable<ulong>(
        ulong.MaxValue,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private NetworkVariable<bool> _door1Open = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> _door2Open = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    void Start()
    {
        _door1Open.OnValueChanged += (old, val) =>
        {
            if (val) door1.Open();
            else door1.Close();
        };

        _door2Open.OnValueChanged += (old, val) =>
        {
            if (val) door2.Open();
            else door2.Close();
        };

        if (warningText != null)
            warningText.gameObject.SetActive(false);
    }

    // Appelé quand un joueur appuie E devant une porte
    [ServerRpc(RequireOwnership = false)]
    public void PlayerAtDoorServerRpc(int doorIndex, ulong clientId)
    {
        Debug.Log("ServerRpc reçu ! Door: " + doorIndex + " Client: " + clientId);
        
        if (doorIndex == 1)
            _playerAtDoor1.Value = clientId;
        else
            _playerAtDoor2.Value = clientId;

        Debug.Log("P1: " + _playerAtDoor1.Value + " P2: " + _playerAtDoor2.Value);
        
        CheckAndOpenDoors();
    }

    void CheckAndOpenDoors()
    {
        ulong p1 = _playerAtDoor1.Value;
        ulong p2 = _playerAtDoor2.Value;

        bool p1Here = p1 != ulong.MaxValue;
        bool p2Here = p2 != ulong.MaxValue;

        Debug.Log("p1Here: " + p1Here + " p2Here: " + p2Here);

        if (p1Here && p2Here && p1 == p2)
        {
            Debug.Log("Même porte !");
            ShowWarningClientRpc("You cannot enter through the same door!");
            return;
        }

        if (p1Here && p2Here)
        {
            Debug.Log("Ouverture des portes !");
            _door1Open.Value = true;
            _door2Open.Value = true;
            StartCoroutine(CloseDoorAfterDelay(3f));
        }
    }

    IEnumerator CloseDoorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _door1Open.Value = false;
        _door2Open.Value = false;

        // Reset
        _playerAtDoor1.Value = ulong.MaxValue;
        _playerAtDoor2.Value = ulong.MaxValue;
    }

    [ClientRpc]
    void ShowWarningClientRpc(string msg)
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(true);
            warningText.text = msg;
            StartCoroutine(HideWarning(3f));
        }
    }

    IEnumerator HideWarning(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (warningText != null)
            warningText.gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerLeftDoorServerRpc(int doorIndex, ulong clientId)
    {
        if (doorIndex == 1 && _playerAtDoor1.Value == clientId)
            _playerAtDoor1.Value = ulong.MaxValue;
        else if (doorIndex == 2 && _playerAtDoor2.Value == clientId)
            _playerAtDoor2.Value = ulong.MaxValue;
    }
}