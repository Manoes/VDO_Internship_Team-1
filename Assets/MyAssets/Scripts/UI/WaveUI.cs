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

        if (PlayerLevelSystem.Instance != null)
        {
            PlayerLevelSystem.Instance.OnLevelChanged.AddListener(UpdateWaveDisplay);
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
            UpdateWaveDisplay(PlayerLevelSystem.Instance.CurrentLevel);
        }

        waveInProgress = true;
        timer = startDelay; // Wait a moment for the first enemies to spawn
    }

    private void OnCombatResumed()
    {
        waveInProgress = true;
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

        // If no enemies are left and the wave was in progress, trigger the next level
        if (enemiesInScene.Length == 0)
        {
            Debug.Log("[WaveUI] All enemies defeated! Transitioning wave...");
            waveInProgress = false;

            if (PlayerLevelSystem.Instance != null)
            {
                // Fill the remaining XP to trigger the level up and upgrade menu
                int xpNeeded = PlayerLevelSystem.Instance.XPToNextLevel - PlayerLevelSystem.Instance.CurrentXP;
                PlayerLevelSystem.Instance.AddXP(xpNeeded);
            }
        }
    }

    public void UpdateWaveDisplay(int level)
    {
        if (waveText != null) waveText.text = wavePrefix + level;

        Debug.Log($"[WaveUI] Wave updated to {level}. Waiting for enemies to be cleared.");
        
        waveInProgress = false; // Stop counting until OnCombatResumed is fired
    }
}
