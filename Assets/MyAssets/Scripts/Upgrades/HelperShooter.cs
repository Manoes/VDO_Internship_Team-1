using UnityEngine;
using UnityEngine.Events;

public class HelperShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Events")]
    public UnityEvent OnShoot;

    [Header("Targeting")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attackRadius = 5f;

    [Header("Shooting")]
    [SerializeField] private float baseFireRate = 2f;
    [SerializeField] private float spawnRadius = 0.5f;

    private float fireTimer;

    private void Update()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer > 0f) return;

        TryShoot();
        float rate = PlayerStats.Instance != null
            ? baseFireRate * PlayerStats.Instance.AttackSpeedMultiplier
            : baseFireRate;
        fireTimer = 1f / Mathf.Max(0.1f, rate);
    }

    private void TryShoot()
    {
        if (projectilePrefab == null) return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRadius, enemyLayer);
        if (enemies.Length == 0) return;

        Collider2D target = enemies[UnityEngine.Random.Range(0, enemies.Length)];
        Vector2 dir       = (target.transform.position - transform.position).normalized;
        Vector2 spawnPos  = (Vector2)transform.position + dir * spawnRadius;

        GameObject proj = PoolRouter.Instance != null
            ? PoolRouter.Instance.GetFromPool(projectilePrefab, spawnPos, Quaternion.identity)
            : Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        if (proj != null && proj.TryGetComponent<Projectile>(out Projectile p))
            p.Initialize(dir);

        OnShoot?.Invoke();
    }
}
