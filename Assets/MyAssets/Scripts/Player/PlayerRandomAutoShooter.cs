using UnityEngine;

public class PlayerRandomAutoShooter : MonoBehaviour
{
    private enum TargetMode
    {
        Closest,
        Strongest,
        Random
    }

    [Header("References")]
    [SerializeField] private PlayerAttackRange attackRange;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Targeting")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private TargetMode targetMode = TargetMode.Random;
    [SerializeField] private float retargetInterval = 1.5f;
    
    [Header("Shooting")]
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float projectileSpawnRadius = 0.5f;

    private float fireTimer;

    private Collider2D currentTarget;
    private float retargetTimer;

    void Awake()
    {
        if(attackRange == null)
            attackRange = GetComponent<PlayerAttackRange>();
    }

    private void Update()
    {
        fireTimer -= Time.deltaTime;

        if(fireTimer <= 0f)
        {
            TryShoot();
            fireTimer = 1f / fireRate;
        }
    }

    private void TryShoot()
    {
        if(attackRange == null) return;
        if(projectilePrefab == null) return;

        Collider2D[] enemiesInsideRange = Physics2D.OverlapCircleAll(
            transform.position,
            attackRange.AttackRadius,
            enemyLayer
        );

        if(enemiesInsideRange.Length == 0) 
        {
            currentTarget = null;
            return;
        }

        retargetTimer -= 1f / fireRate;

        if(currentTarget == null || retargetTimer <= 0f || !IsTargetInsideRange(currentTarget) || !currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = GetTarget(enemiesInsideRange);
            retargetTimer = retargetInterval;
        }

        if(currentTarget == null) return;

        Vector2 direction = (currentTarget.transform.position - transform.position).normalized;

        Vector2 spawnPosition =
            (Vector2)transform.position +
            direction * projectileSpawnRadius;
        
        GameObject projectile = PoolRouter.Instance != null
            ? PoolRouter.Instance.GetFromPool(projectilePrefab, spawnPosition, Quaternion.identity)
            : Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        
         if (projectile == null) return;

        if (projectile.TryGetComponent<Projectile>(out Projectile projectileObject))
            projectileObject.Initialize(direction);
    }

    private bool IsTargetInsideRange(Collider2D target)
    {
        if(target == null) return false;

        float distanceSqr = (transform.position - target.transform.position).sqrMagnitude;
        return distanceSqr <= attackRange.AttackRadius * attackRange.AttackRadius;
    }

    private Collider2D GetTarget(Collider2D[] enemies)
    {
        if(enemies == null || enemies.Length == 0) return null;

        switch (targetMode)
        {
            case TargetMode.Closest:
                return GetClosestTarget(enemies);

            case TargetMode.Strongest:
                return GetStrongestTarget(enemies);

            case TargetMode.Random:
                return enemies[Random.Range(0, enemies.Length)];

            default:
                return enemies[0];
        }
    }

    private Collider2D GetStrongestTarget(Collider2D[] enemies)
    {
        Collider2D strongest = null;
        int highestHealth = -1;

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.TryGetComponent<EnemyHealth>(out EnemyHealth health))
            {
                if (health.CurrentHealth > highestHealth)
                {
                    highestHealth = health.CurrentHealth;
                    strongest = enemy;
                }
            }
        }

        return strongest != null ? strongest : GetClosestTarget(enemies);
    }

    private Collider2D GetClosestTarget(Collider2D[] enemies)
    {
        Collider2D closest = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (Collider2D enemy in enemies)
        {
            float distanceSqr = (transform.position - enemy.transform.position).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closest = enemy;
            }
        }

        return closest;
    }

    private void OnDrawGizmosSelected()
    {
        if(attackRange == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange.AttackRadius);
    }
}
