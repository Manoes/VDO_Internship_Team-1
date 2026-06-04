using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image           iconImage;
    [SerializeField] private Button          button;

    private UpgradeDefinition         currentUpgrade;
    private Action<UpgradeDefinition> onSelected;

    public void Initialize(UpgradeDefinition def, Action<UpgradeDefinition> callback)
    {
        currentUpgrade = def;
        onSelected     = callback;

        if (nameText        != null) nameText.text = def.UpgradeName;
        if (descriptionText != null) descriptionText.text = def.Description;
        if (iconImage       != null)
        {
            iconImage.sprite = def.Icon;
            iconImage.gameObject.SetActive(def.Icon != null);
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        Debug.Log($"[UpgradeCard] Clicked: {currentUpgrade?.UpgradeName}");
        onSelected?.Invoke(currentUpgrade);
    }
}