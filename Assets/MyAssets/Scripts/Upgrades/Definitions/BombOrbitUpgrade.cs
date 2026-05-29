using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Bomb Orbit", fileName = "BombOrbitUpgrade")]
public class BombOrbitUpgrade : UpgradeDefinition
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private int   bombCount   = 3;
    [SerializeField] private float orbitRadius = 2.5f;
    [SerializeField] private float orbitSpeed = 90f;
    
    public override void Apply(PlayerStats stats)
    {
        for (int i = 0; i < bombCount; i++)
        {
            // evenly space bombs around the orbit from the start
            float angle = i * (360f / bombCount);

            GameObject bomb = Object.Instantiate(bombPrefab, stats.transform.position, Quaternion.identity);
            bomb.GetComponent<HelperOrbit>().Initialize(stats.transform, angle, orbitRadius, orbitSpeed);
        }
    }
}
