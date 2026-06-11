using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Vivox;
using Unity.Netcode;
using UnityEngine;
using System.Threading.Tasks;

public class VoiceChatManager : MonoBehaviour
{
    public static VoiceChatManager Instance { get; private set; }

    private const string ChannelName = "GameChannel";
    public bool IsVivoxReady { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
            await UnityServices.InitializeAsync();

        // Attendre que l'authentification soit terminée (gérée par un autre script)
        while (!AuthenticationService.Instance.IsSignedIn)
        {
            await Task.Delay(100);
        }

        await VivoxService.Instance.InitializeAsync();
        IsVivoxReady = true;

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private async void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            await JoinVoiceChannel();
        }
    }

    private async void OnClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            await VivoxService.Instance.LeaveAllChannelsAsync();
        }
    }

    private async Task JoinVoiceChannel()
    {
        await VivoxService.Instance.JoinGroupChannelAsync(ChannelName, ChatCapability.AudioOnly);
        Debug.Log("Joined voice channel: " + ChannelName);
    }

    public void ToggleMicrophone(bool micEnabled)
    {
        if (!IsVivoxReady) return;

        if (micEnabled)
            VivoxService.Instance.UnmuteInputDevice();
        else
            VivoxService.Instance.MuteInputDevice();
    }

    private void OnDestroy()
    {
        if (Instance == this && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}