using Unity.Mathematics;
using UnityEngine;

public class ProjectileDamageApplier : MonoBehaviour
{
    private Projectile projectile;
    private int baseDamage; // cached once at Awake, the inspector value

    private void Awake()
    {
        projectile = GetComponent<Projectile>();
        if (projectile != null)
            baseDamage = ReflectionHelper.GetField<int>(projectile, "damage");
    }

    private void OnEnable()
    {
        // fires every time this projectile is taken from pool (SetActive true)
        if (projectile == null || PlayerStats.Instance == null) return;
        int scaled = Mathf.Max(1, Mathf.RoundToInt(baseDamage * PlayerStats.Instance.DamageMultiplier));
        ReflectionHelper.SetField(projectile, "damage", scaled);
    }
}
