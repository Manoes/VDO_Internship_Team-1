using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeManager : Singleton<UpgradeManager>
{
    [SerializeField] private UpgradeDefinition[] upgradePool; // drag all upgrade SO's here
    [SerializeField] private UpgradeUI           upgradeUI;

    public UnityEvent OnUpgradeSelected;
    private Action     onSelectionComplete;

    public void TriggerUpgradeSelection()
    {
        if (upgradeUI == null)
        {
            Debug.LogError("[UpgradeManager] UpgradeUI reference is missing in the Inspector!");
            return;
        }

        upgradeUI.Show(PickRandom(3), OnUpgradePicked);
        Time.timeScale = 0f; // pause the game while selecting
    }

    private void OnUpgradePicked(UpgradeDefinition chosenUpgrade)
    {
        if (PlayerStats.Instance != null)
            chosenUpgrade.Apply(PlayerStats.Instance);

        // Update the visual tracker for owned upgrades
        if (OwnedUpgradesUI.Instance != null)
            OwnedUpgradesUI.Instance.AddUpgrade(chosenUpgrade.UpgradeName, chosenUpgrade.Icon);

        upgradeUI.Hide();
        Time.timeScale = 1f; // resume the game

        OnUpgradeSelected?.Invoke();
    }

    private UpgradeDefinition[] PickRandom(int count)
    {
        if (upgradePool == null || upgradePool.Length == 0) return Array.Empty<UpgradeDefinition>();

        List<UpgradeDefinition> pool = new(upgradePool);

        // if pool is too small, allow duplicates
        if (pool.Count <= count)
        {
            var dupes = new UpgradeDefinition[count];
            for (int i = 0; i < count; i++) dupes[i] = pool[UnityEngine.Random.Range(0, pool.Count)];
            return dupes;
        }

        // Fisher-Yates partial shuffle to get unique random picks
        for (int i = 0; i < count; i++)
        {
            int j = UnityEngine.Random.Range(i, pool.Count);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        var result = new UpgradeDefinition[count];
        pool.CopyTo(0, result, 0, count);
        return result;
    }
}
