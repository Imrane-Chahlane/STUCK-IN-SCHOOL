using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject menuPanel;
    public TMP_Text joinCodeText;
    public TMP_InputField codeInput;
    public Button btnCreer;
    public Button btnRejoindre;

    async void Start()
    {
        btnCreer.interactable = false;
        btnRejoindre.interactable = false;

        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance
                      .SignInAnonymouslyAsync();

            Debug.Log("Connecté ! ID : " +
                      AuthenticationService.Instance.PlayerId);

            btnCreer.interactable = true;
            btnRejoindre.interactable = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur Auth : " + e.Message);
        }
    }

    public async void CreateGame()
    {
        try
        {
            Allocation allocation = await RelayService.Instance
                .CreateAllocationAsync(2);

            string joinCode = await RelayService.Instance
                .GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton
                .GetComponent<UnityTransport>()
                .SetHostRelayData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData
                );

            joinCodeText.text = "Code : " + joinCode;
            menuPanel.SetActive(false);
            NetworkManager.Singleton.StartHost();

            Debug.Log("Partie créée ! Code : " + joinCode);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur création : " + e.Message);
        }
    }

    public async void JoinGame()
    {
        try
        {
            string code = codeInput.text.Trim().ToUpper();

            JoinAllocation joinAllocation = await RelayService.Instance
                .JoinAllocationAsync(code);

            NetworkManager.Singleton
                .GetComponent<UnityTransport>()
                .SetClientRelayData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.Key,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData
                );

            menuPanel.SetActive(false);
            NetworkManager.Singleton.StartClient();

            Debug.Log("Connecté !");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur connexion : " + e.Message);
        }
    }
}