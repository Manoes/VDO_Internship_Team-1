using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Damage", fileName = "DamageUpgrade")]
public class DamageUpgrade : UpgradeDefinition
{
    [SerializeField, Range(0.1f, 1f)] private float bonusPercent = 0.25f;
    public override void Apply(PlayerStats stats) => stats.AddDamage(bonusPercent);
}
