using System;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private UpgradeCard[] cards;

    public UpgradeDefinition[] CurrentChoices { get; private set; }
    public Action<UpgradeDefinition> CurrentCallback { get; private set; }

    public void Show(UpgradeDefinition[] choices, Action<UpgradeDefinition> onSelect)
    {
        CurrentChoices = choices;
        CurrentCallback = onSelect;

        panel.SetActive(true);

        for (int i = 0; i < cards.Length; i++)
        {
            bool hasChoice = i < choices.Length;
            cards[i].gameObject.SetActive(hasChoice);

            if (hasChoice)
                cards[i].Initialize(choices[i], onSelect);
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

    public void Hide() => panel.SetActive(false);
}