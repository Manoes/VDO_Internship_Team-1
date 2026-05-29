using System;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private GameObject    panel;
    [SerializeField] private UpgradeCard[] cards;

    public void Show(UpgradeDefinition[] choices, Action<UpgradeDefinition> onSelect)
    {
        panel.SetActive(true);
        for (int i = 0; i < cards.Length; i++)
        {
            bool hasChoice = i < choices.Length;
            cards[i].gameObject.SetActive(hasChoice);
            if (hasChoice) cards[i].Initialize(choices[i], onSelect);
        }
    }
    
    public void Hide() => panel.SetActive(false);
}