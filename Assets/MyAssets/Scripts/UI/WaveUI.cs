using UnityEngine;
using TMPro;

public class WaveUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Settings")]
    [SerializeField] private string wavePrefix = "Wave: ";

    private void Start()
    {
        if (PlayerLevelSystem.Instance != null)
        {
            // Subscribing here ensures the Singleton Instance is actually ready
            PlayerLevelSystem.Instance.OnLevelChanged.AddListener(UpdateWaveDisplay);
            UpdateWaveDisplay(PlayerLevelSystem.Instance.CurrentLevel);
        }
    }    

    public void UpdateWaveDisplay(int level)
    {
        if (waveText != null) 
            waveText.text = wavePrefix + level;
    }
}
