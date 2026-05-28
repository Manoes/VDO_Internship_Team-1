using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Attack Speed", fileName = "AttackSpeedUpgrade")]
public class AttackSpeedUpgrade : UpgradeDefinition
{
    [SerializeField, Range(0.05f, 1f)] private float bonusPercent = 0.25f;
    public override void Apply(PlayerStats stats) => stats.AddAttackSpeed(bonusPercent);
}
