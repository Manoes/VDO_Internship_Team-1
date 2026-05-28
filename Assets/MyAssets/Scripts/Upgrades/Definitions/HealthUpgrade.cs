using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Health & Regen", fileName = "HealthUpgrade")]
public class HealthUpgrade : UpgradeDefinition
{
    [SerializeField] private int   bonusMaxHealth = 2;
    [SerializeField] private float regenPerSecond = 0.5f;

    public override void Apply(PlayerStats stats)
    {
        if (bonusMaxHealth > 0)  stats.AddMaxHealth(bonusMaxHealth);
        if (regenPerSecond > 0f) stats.AddRegen(regenPerSecond);
    }
}