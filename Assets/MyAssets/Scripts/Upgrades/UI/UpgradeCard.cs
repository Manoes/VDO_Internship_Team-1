using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    [Header("Animation")]
    [SerializeField] private RectTransform cardRect;
    [SerializeField] private float slideDistance = 150f;
    [SerializeField] private float slideDuration = 0.35f;

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

    public void PlayIntroAnimation(float delay)
    {
        cardRect.DOKill();

        Vector2 finalPosition = cardRect.anchoredPosition;

        cardRect.anchoredPosition = finalPosition + Vector2.down * slideDistance;

        cardRect.localScale = Vector2.one * 0.8f;

        Sequence sequence = DOTween.Sequence().SetUpdate(true);

        sequence.AppendInterval(delay);

        sequence.Append(
            cardRect
                .DOAnchorPos(finalPosition, slideDuration)
                .SetEase(Ease.OutBack)
        );
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