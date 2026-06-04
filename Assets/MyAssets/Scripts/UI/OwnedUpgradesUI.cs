using UnityEngine;
using System.Collections.Generic;

public class OwnedUpgradesUI : MonoBehaviour
{
    public static OwnedUpgradesUI Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private GameObject itemPrefab; // The prefab with OwnedUpgradeItem attached
    [SerializeField] private Transform container;    // Drag your Vertical Layout Group here

    private Dictionary<string, OwnedUpgradeItem> activeUpgrades = new Dictionary<string, OwnedUpgradeItem>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // If container isn't set, default to this transform
        if (container == null) container = transform;
    }

    /// <summary>
    /// Call this whenever an upgrade is selected.
    /// </summary>
    /// <param name="upgradeName">Unique name of the upgrade to track stacks.</param>
    /// <param name="icon">The icon to display if it's the first time picking it.</param>
    public void AddUpgrade(string upgradeName, Sprite icon)
    {
        if (activeUpgrades.ContainsKey(upgradeName))
        {
            // We already have it, just increase the number
            activeUpgrades[upgradeName].Increment();
        }
        else
        {
            // First time picking this, spawn the icon
            GameObject newItem = Instantiate(itemPrefab, container);
            OwnedUpgradeItem itemScript = newItem.GetComponent<OwnedUpgradeItem>();
            itemScript.Setup(icon);
            activeUpgrades.Add(upgradeName, itemScript);
        }
    }
}