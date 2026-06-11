using UnityEngine;
using UnityEngine.UI;

public class MicToggleUI : MonoBehaviour
{
    public Toggle micToggle;

    private void Start()
    {
        if (micToggle != null)
        {
            micToggle.isOn = true; // micro activé par défaut
            micToggle.onValueChanged.AddListener(OnToggle);
        }
    }

    private void OnToggle(bool micEnabled)
    {
        if (VoiceChatManager.Instance != null)
        {
            VoiceChatManager.Instance.ToggleMicrophone(micEnabled);
        }
    }
}