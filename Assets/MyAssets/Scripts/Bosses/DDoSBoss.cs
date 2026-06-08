using System.Collections;
using UnityEngine;

public class DDoSBoss : BossBase
{
    private enum AttackPattern
    {
        RadialBurst,
        FourWayFan,
        BulletWall
    }

    [Header("Audio")]
    [SerializeField] private AudioSource shootAudio;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpawnRadius = 1f;

    [Header("Timing")]
    [SerializeField] private float baseAttackInterval = 1.5f;
    [SerializeField] private float minAttackInterval = 0.5f;
    [SerializeField] private float intervalDecreasePerLevel = 0.05f;

    [Header("Radial Burst")]
    [SerializeField] private int radialBurstWaves = 3;
    [SerializeField] private int baseRadialBulletCount = 12;
    [SerializeField] private int extraRadialBulletsEveryXLevels = 5;
    [SerializeField] private float radialBurstWaveDelay = 0.15f;
    [SerializeField] private float radialWaveRotationOffset = 15f;
    [SerializeField] private float radialRotationSpeed = 20f;

    [Header("Four Way Fan")]
    [SerializeField] private int fanWaves = 3;
    [SerializeField] private int fanBulletsPerDirection = 5;
    [SerializeField] private float fanAngle = 45f;
    [SerializeField] private float fanWaveDelay = 0.12f;
    [SerializeField] private float fanWaveAngleStep = 8f;

    [Header("Bullet Wall")]
    [SerializeField] private int wallWaves = 3;
    [SerializeField] private int wallBulletCount = 9;
    [SerializeField] private float wallSpacing = 0.75f;
    [SerializeField] private float wallWaveDelay = 0.12f;
    [SerializeField] private float wallSideOffsetPerWave = 0.4f;

    private bool isAttacking;
    private float attackTimer;
    private float radialAngleOffset;
    private int currentAttackIndex;

    private void Update()
    {
        radialAngleOffset += radialRotationSpeed * Time.deltaTime;

        if (isAttacking) return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            PerformNextAttack();
            attackTimer = GetAttackInterval();
        }
    }

    private void PerformNextAttack()
    {
        AttackPattern pattern = (AttackPattern)currentAttackIndex;

        switch (pattern)
        {
            case AttackPattern.RadialBurst:
                StartCoroutine(FireRadialBurst());
                break;

            case AttackPattern.FourWayFan:
                StartCoroutine(FireFourWayFan());
                break;

            case AttackPattern.BulletWall:
                StartCoroutine(FireBulletWall());
                break;
        }

        currentAttackIndex++;

        if (currentAttackIndex >= System.Enum.GetValues(typeof(AttackPattern)).Length)
            currentAttackIndex = 0;
    }

    private IEnumerator FireRadialBurst()
    {
        isAttacking = true;

        int bulletCount =
            baseRadialBulletCount +
            bossLevel / extraRadialBulletsEveryXLevels;
        
        float angleStep = 360f / bulletCount;

        for (int wave = 0; wave < radialBurstWaves; wave++)
        {
            shootAudio?.PlayOneShot(shootAudio.clip);

            float waveOffset =
                radialAngleOffset +
                wave * radialWaveRotationOffset;

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = radialAngleOffset + angleStep * i;
                FireProjectile(angle);
            }

            yield return new WaitForSeconds(radialBurstWaveDelay);
        }

        isAttacking = false;
    }

    private IEnumerator FireFourWayFan()
    {
        isAttacking = true;

        float[] baseAngles = { 0f, 90f, 180f, 270f };

        for (int wave = 0; wave < fanWaves; wave++)
        {
            shootAudio?.PlayOneShot(shootAudio.clip);

            float waveOffset = Mathf.Sin(wave * fanWaveAngleStep * Mathf.Deg2Rad) * fanAngle;

            foreach (float baseAngle in baseAngles)
            {
                for (int i = 0; i < fanBulletsPerDirection; i++)
                {
                    float t = fanBulletsPerDirection == 1
                        ? 0.5f
                        : i / (float)(fanBulletsPerDirection - 1);

                    float spreadOffset = Mathf.Lerp(-fanAngle * 0.5f, fanAngle * 0.5f, t);

                    FireProjectile(baseAngle + spreadOffset + waveOffset);
                }
            }

            yield return new WaitForSeconds(fanWaveDelay);
        }

        isAttacking = false;
    }

    private IEnumerator FireBulletWall()
    {
        isAttacking = true;

        if (projectilePrefab == null)
        {
            isAttacking = false;
            yield break;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            isAttacking = false;
            yield break;
        }

        Vector2 direction = (playerObject.transform.position - transform.position).normalized;

        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        float startOffset = -(wallBulletCount - 1) * wallSpacing * 0.5f;

        for (int wave = 0; wave < wallWaves; wave++)
        {
            shootAudio?.PlayOneShot(shootAudio.clip);

            float waveSideOffset = Mathf.Sin(wave * Mathf.PI * 0.5f) * wallSideOffsetPerWave;

            for (int i = 0; i < wallBulletCount; i++)
            {
                Vector2 spawnPosition =
                    (Vector2)transform.position +
                    perpendicular * (startOffset + i * wallSpacing);

                SpawnProjectile(spawnPosition, direction);
            }

            yield return new WaitForSeconds(wallWaveDelay);
        }

        isAttacking = false;
    }

    private void FireProjectile(float angle)
    {
        Vector2 direction = AngleToDirection(angle);

        Vector2 spawnPosition =
            (Vector2)transform.position +
            direction * projectileSpawnRadius;

        SpawnProjectile(spawnPosition, direction);
    }

    private void SpawnProjectile(Vector2 spawnPosition, Vector2 direction)
    {
        if (projectilePrefab == null) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        GameObject projectile = PoolRouter.Instance != null
            ? PoolRouter.Instance.GetFromPool(projectilePrefab, spawnPosition, rotation)
            : Instantiate(projectilePrefab, spawnPosition, rotation);

        if (projectile == null) return;

        if (projectile.TryGetComponent<Projectile>(out Projectile projectileObject))
            projectileObject.Initialize(direction);
    }

    private float GetAttackInterval()
    {
        float interval = baseAttackInterval - bossLevel * intervalDecreasePerLevel;
        return Mathf.Max(interval, minAttackInterval);
    }

    private Vector2 AngleToDirection(float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
    }
}