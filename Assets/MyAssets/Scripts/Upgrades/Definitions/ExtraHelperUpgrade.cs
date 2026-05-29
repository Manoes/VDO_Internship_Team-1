using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Extra Helper", fileName = "ExtraHelperUpgrade")]
public class ExtraHelperUpgrade : UpgradeDefinition
{
    public override void Apply(PlayerStats stats) => stats.AddHelper();
}
