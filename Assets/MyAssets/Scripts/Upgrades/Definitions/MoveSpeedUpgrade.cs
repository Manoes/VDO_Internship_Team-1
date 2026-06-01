using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Move Speed", fileName = "MoveSpeedUpgrade")]
public class MoveSpeedUpgrade : UpgradeDefinition
{
    [SerializeField, Range(0.05f, 1f)] private float bonusPercent = 0.2f;
    public override void Apply(PlayerStats stats) => stats.AddMoveSpeed(bonusPercent);
}
