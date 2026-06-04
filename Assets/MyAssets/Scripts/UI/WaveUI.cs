using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class WaveUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Settings")]
    [SerializeField] private string wavePrefix = "Wave: ";
    [SerializeField] private float checkInterval = 0.5f;
    [SerializeField] private float startDelay = 2f;
    
    private float timer;
    private bool waveInProgress = false;

    private void OnEnable()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnCombatResumed.AddListener(OnCombatResumed);
        }
    }

    private void OnDisable()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnCombatResumed.RemoveListener(OnCombatResumed);
        }

        if (PlayerLevelSystem.Instance != null)
        {
            PlayerLevelSystem.Instance.OnLevelChanged.RemoveListener(UpdateWaveDisplay);
        }
    }

    private void Start()
    {
        if (PlayerLevelSystem.Instance != null)
        {
            // Subscribing here ensures the Singleton Instance is actually ready
            PlayerLevelSystem.Instance.OnLevelChanged.AddListener(UpdateWaveDisplay);
            UpdateWaveDisplay(PlayerLevelSystem.Instance.CurrentLevel);
        }
        else
        {
            Debug.LogError("[WaveUI] PlayerLevelSystem instance not found!");
        }

        waveInProgress = true;
        timer = startDelay; // Wait a moment for the first enemies to spawn
    }

    private void OnCombatResumed()
    {
        waveInProgress = true;
        timer = startDelay; // Reset timer so we don't instantly trigger the next wave
    }

    private void Update()
    {
        if (!waveInProgress) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = checkInterval;
            CheckWaveCompletion();
        }
    }

    private void CheckWaveCompletion()
    {
        // Look for any objects tagged "Enemy" currently in the scene
        GameObject[] enemiesInScene = GameObject.FindGameObjectsWithTag("Enemy");

        // If no enemies are left and the wave was in progress, trigger the transition
        if (enemiesInScene.Length == 0 && waveInProgress)
        {
            TriggerWaveTransition();
        }
    }

    public void TriggerWaveTransition()
    {
        if (!waveInProgress) return; // Prevent double-triggering

        Debug.Log("[WaveUI] All enemies defeated! Transitioning wave...");
        waveInProgress = false;

        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.TriggerUpgradeSelection(StartNextWave);
        }
        else
        {
            Debug.LogError("[WaveUI] UpgradeManager.Instance is missing! Did you add the script to a GameObject?");
            StartNextWave();
        }
    }

    public void StartNextWave()
    {
        if (PlayerLevelSystem.Instance != null)
        {
            // Fill the remaining XP to trigger the level up and update the display
            int xpNeeded = PlayerLevelSystem.Instance.XPToNextLevel - PlayerLevelSystem.Instance.CurrentXP;
            PlayerLevelSystem.Instance.AddXP(xpNeeded);
        }
    }

    public void UpdateWaveDisplay(int level)
    {
        if (waveText != null) waveText.text = wavePrefix + level;
        Debug.Log($"[WaveUI] Wave display updated to {level}.");

        // REMOVED: waveInProgress = false; 
        // We only want to set waveInProgress to false inside TriggerWaveTransition.
        // If the player levels up via XP gems mid-wave, we must keep checking for enemies!
    }
}
