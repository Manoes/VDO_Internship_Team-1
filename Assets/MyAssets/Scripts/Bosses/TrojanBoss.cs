using UnityEngine;

public class TrojanBoss : BossBase
{
    [Header("Enemy Spawning")]
    [SerializeField] private GameObject tinyEnemyPrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnRadius = 1.5f;

    [Header("Scaling")]
    [SerializeField] private int baseSpawnAmount = 6;
    [SerializeField] private int extraSpawnEveryXLevels = 5;
    [SerializeField] private float minSpawnInterval = 0.6f;
    [SerializeField] private float intervalDecreaseEveryXLevels = 0.05f;

    private float spawnTimer;

    public override void Initialize(int level)
    {
        base.Initialize(level);
        spawnTimer = GetSpawnInterval();
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnTinyEnemies();
            spawnTimer = GetSpawnInterval();
        }
    }

    public void SpawnTinyEnemies()
    {
        if (tinyEnemyPrefab == null) return;

        int amount = baseSpawnAmount + bossLevel / extraSpawnEveryXLevels;

        for (int i = 0; i < amount; i++)
        {
            Vector2 randOffset = Random.insideUnitCircle.normalized * spawnRadius;
            Vector2 spawnPosition = (Vector2)transform.position + randOffset;

            if (PoolRouter.Instance != null)
                PoolRouter.Instance.GetFromPool(tinyEnemyPrefab, spawnPosition, Quaternion.identity);
            else
                Instantiate(tinyEnemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private float GetSpawnInterval()
    {
        float interval = spawnInterval - bossLevel * intervalDecreaseEveryXLevels;
        return Mathf.Max(interval, minSpawnInterval);
    }
}
