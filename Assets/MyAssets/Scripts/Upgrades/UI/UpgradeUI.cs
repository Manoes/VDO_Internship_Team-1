using System;
using DG.Tweening;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private UpgradeCard[] cards;

    [Header("Animation")]
    [SerializeField] private float panelPopDuration = 0.25f;
    [SerializeField] private float cardDelay = 0.1f;

    public UpgradeDefinition[] CurrentChoices { get; private set; }
    public Action<UpgradeDefinition> CurrentCallback { get; private set; }

    public void Show(UpgradeDefinition[] choices, Action<UpgradeDefinition> onSelect)
    {
        CurrentChoices = choices;
        CurrentCallback = onSelect;

        panel.SetActive(true);

        panelRect.localScale = Vector3.zero;

        panelRect
            .DOScale(1.3f, panelPopDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        for (int i = 0; i < cards.Length; i++)
        {
            bool hasChoice = i < choices.Length;
            cards[i].gameObject.SetActive(hasChoice);

            if(!hasChoice)
                continue;

            cards[i].Initialize(choices[i], onSelect);
            cards[i].PlayIntroAnimation(i * cardDelay);
        }
    }

    public void PickByIndex(int index)
    {
        Debug.Log($"PickByIndex called: {index}");

        if (CurrentChoices == null) return;
        if (CurrentCallback == null) return;
        if (index < 0 || index >= CurrentChoices.Length) return;

        Action<UpgradeDefinition> callback = CurrentCallback;
        UpgradeDefinition choice = CurrentChoices[index];

        CurrentCallback = null;
        CurrentChoices = null;

        callback?.Invoke(choice);
    }

    public void Hide() 
    {
        panelRect
            .DOScale(0f, 0.15f)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                panel.SetActive(false);
            });
    }
}