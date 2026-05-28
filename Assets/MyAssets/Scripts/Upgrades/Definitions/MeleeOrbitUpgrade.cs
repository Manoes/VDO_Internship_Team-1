using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Melee Orbit", fileName = "MeleeOrbitUpgrade")]
public class MeleeOrbitUpgrade : UpgradeDefinition
{
    [SerializeField] private GameObject meleeWeaponPrefab;
    [SerializeField] private float orbitRadius = 1.2f;
    [SerializeField] private float orbitSpeed = 150f;
    
    public override void Apply(PlayerStats stats)
    {
        GameObject weapon = Object.Instantiate(meleeWeaponPrefab, stats.transform.position, Quaternion.identity);

        // random start angle so multiple melee pickups don't stack at the same spot
        float startAngle = Random.Range(0f, 360f);

        weapon.GetComponent<HelperOrbit>().Initialize(stats.transform, startAngle, orbitRadius, orbitSpeed);
    }
}
