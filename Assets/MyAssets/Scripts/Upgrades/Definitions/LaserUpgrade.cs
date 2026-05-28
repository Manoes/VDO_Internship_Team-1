using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Laser", fileName = "LaserUpgrade")]
public class LaserUpgrade : UpgradeDefinition
{
    [SerializeField] private GameObject laserPrefab;

    public override void Apply(PlayerStats stats)
    {
        GameObject laser = Object.Instantiate(laserPrefab, stats.transform.position, Quaternion.identity);
        laser.GetComponent<LaserOrbitWeapon>().Initialize(stats.transform);
    }
}