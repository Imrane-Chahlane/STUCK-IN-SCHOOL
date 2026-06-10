using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;

public class VoiceTelecomManager : MonoBehaviour
{
    public static VoiceTelecomManager Instance { get; private set; }

    [Header("Voice Channel")]
    [SerializeField] private string defaultChannelName = "stuck-in-school-global-voice";

    [Header("Auto Start")]
    [SerializeField] private bool connectOnStart = true;

    private bool isInitialized;
    private bool isConnected;
    private string currentChannelName;

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (connectOnStart)
        {
            await ConnectToVoiceAsync(defaultChannelName);
        }
    }

    public async Task ConnectToVoiceAsync(string channelName)
    {
        if (isConnected)
            return;

        currentChannelName = string.IsNullOrWhiteSpace(channelName)
            ? defaultChannelName
            : channelName;

        try
        {
            await InitializeVivoxAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            await VivoxService.Instance.LoginAsync();

            await VivoxService.Instance.JoinGroupChannelAsync(
                currentChannelName,
                ChatCapability.AudioOnly
            );

            isConnected = true;
            Debug.Log($"[VoiceTelecom] Connected to voice channel: {currentChannelName}");
        }
        catch (Exception exception)
        {
            Debug.LogError($"[VoiceTelecom] Voice connection failed: {exception.Message}");
        }
    }

    public async Task DisconnectVoiceAsync()
    {
        if (!isConnected)
            return;

        try
        {
            await VivoxService.Instance.LeaveAllChannelsAsync();
            await VivoxService.Instance.LogoutAsync();

            isConnected = false;
            Debug.Log("[VoiceTelecom] Disconnected from voice.");
        }
        catch (Exception exception)
        {
            Debug.LogError($"[VoiceTelecom] Voice disconnect failed: {exception.Message}");
        }
    }

    public void SetMicrophoneMuted(bool muted)
    {
        VivoxService.Instance.SetChannelTransmissionModeAsync(
            muted ? TransmissionMode.None : TransmissionMode.All
        );

        Debug.Log($"[VoiceTelecom] Microphone muted: {muted}");
    }

    private async Task InitializeVivoxAsync()
    {
        if (isInitialized)
            return;

        await UnityServices.InitializeAsync();
        await VivoxService.Instance.InitializeAsync();

        isInitialized = true;
        Debug.Log("[VoiceTelecom] Vivox initialized.");
    }

    private async void OnApplicationQuit()
    {
        await DisconnectVoiceAsync();
    }
}