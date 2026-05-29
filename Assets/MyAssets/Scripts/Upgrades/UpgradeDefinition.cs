using UnityEngine;

public abstract class UpgradeDefinition : ScriptableObject
{
    [SerializeField]           private string upgradeName;
    [SerializeField, TextArea] private string description;
    [SerializeField]           private Sprite icon;

    public string UpgradeName  => upgradeName;
    public string Description  => description;
    public Sprite Icon         => icon;

    public abstract void Apply(PlayerStats stats);
}
