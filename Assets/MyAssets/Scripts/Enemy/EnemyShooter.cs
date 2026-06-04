using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform firePointPivot;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Targeting")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Shooting")]
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float fireRateIncreasePerLevel = 0.05f;
    [SerializeField] private float maxFireRate = 5f;
    [SerializeField] private float projectileSpawnDistance = 0.75f;

    private float baseFireRate;
    private Transform player;
    private float fireTimer;

    void Awake()
    {
        baseFireRate = fireRate;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if(playerObject != null)
            player = playerObject.transform;
    }

    void Update()
    {
        if(player == null) return;

        Vector2 direction = player.position - transform.position;

        if(direction.magnitude > detectionRange)
            return;
        
        RotateFirepoint(direction);

        fireTimer -= Time.deltaTime;

        if(fireTimer <= 0f)
        {
            Shoot(direction.normalized);
            fireTimer = 1f / fireRate;
        }
    }

    public void ApplyLevelScaling(int level)
    {
        fireRate = Mathf.Min(baseFireRate + (level - 1) * fireRateIncreasePerLevel, maxFireRate);
    }

    private void RotateFirepoint(Vector2 direction)
    {
        if (firePointPivot == null) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        firePointPivot.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void Shoot(Vector2 direction)
    {
        Vector2 spawnPosition = firePoint != null
            ? firePoint.position
            : (Vector2)transform.position + 
              direction * projectileSpawnDistance;

        Quaternion rotation = 
            Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
        
        GameObject projectile = PoolRouter.Instance != null 
            ? PoolRouter.Instance.GetFromPool(projectilePrefab, spawnPosition, rotation)
            : Instantiate(projectilePrefab, spawnPosition, rotation);
        
        if(projectile == null) return;

        if(projectile.TryGetComponent<Projectile>(out Projectile projectileObject))
            projectileObject.Initialize(direction);
    }
}
