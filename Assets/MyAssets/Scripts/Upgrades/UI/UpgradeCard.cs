using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    private UpgradeDefinition currentUpgrade;
    private Action<UpgradeDefinition> onSelected;

    public void Initialize(UpgradeDefinition def, Action<UpgradeDefinition> callback)
    {
        Debug.Log($"[UpgradeCard] Initialize called on {gameObject.name}");

        currentUpgrade = def;
        onSelected = callback;

        if (nameText != null) nameText.text = def.UpgradeName;
        if (descriptionText != null) descriptionText.text = def.Description;
        if (iconImage != null)
        {
            iconImage.sprite = def.Icon;
            iconImage.gameObject.SetActive(def.Icon != null);
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(HandleClick);

        Debug.Log($"[UpgradeCard] Listener added on {gameObject.name}");
    }

    private void HandleClick()
    {
        Debug.Log($"[UpgradeCard] Clicked: {currentUpgrade?.UpgradeName}");

        if (currentUpgrade == null)
        {
            Debug.LogError("[UpgradeCard] currentUpgrade is null.");
            return;
        }

        if (onSelected == null)
        {
            Debug.LogError("[UpgradeCard] onSelected callback is null.");
            return;
        }

        onSelected?.Invoke(currentUpgrade);
    }
}