using UnityEngine;

[CreateAssetMenu(fileName = "Upgrades/Extra Range", menuName = "ExtraRangeUpgrade")]
public class AttackRangeUpgrade : UpgradeDefinition
{
    [SerializeField, Range(0.05f,1f)] private float bonusPercent = 0.25f;

    public override void Apply(PlayerStats stats)
    {
        stats.AddRange(bonusPercent);
    }
}
