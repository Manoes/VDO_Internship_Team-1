using UnityEngine;
using UnityEngine.Events;

public class PlayerRandomAutoShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAttackRange attackRange;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Events")]
    public UnityEvent OnShoot;

    [Header("Targeting")]
    [SerializeField] private LayerMask enemyLayer;
    
    [Header("Shooting")]
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float projectileSpawnRadius = 0.5f;

    private float fireTimer;

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

        if(enemiesInsideRange.Length == 0) return;

        Collider2D randomEnemy = enemiesInsideRange[Random.Range(0, enemiesInsideRange.Length)];

        Vector2 direction = (randomEnemy.transform.position - transform.position).normalized;

        Vector2 spawnPosition =
            (Vector2)transform.position +
            direction * projectileSpawnRadius;
        
        GameObject projectile = PoolRouter.Instance != null
            ? PoolRouter.Instance.GetFromPool(projectilePrefab, spawnPosition, Quaternion.identity)
            : Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        
         if (projectile == null) return;

        if (projectile.TryGetComponent<Projectile>(out Projectile projectileObject))
            projectileObject.Initialize(direction);

        OnShoot?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if(attackRange == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange.AttackRadius);
    }
}
