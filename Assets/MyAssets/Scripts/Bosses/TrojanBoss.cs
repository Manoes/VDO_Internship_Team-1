using UnityEngine;

public class TrojanBoss : BossBase
{
    [Header("Split On Death")]
    [SerializeField] private GameObject tinyEnemyPrefab;
    [SerializeField] private int baseSpawnAmount = 6;
    [SerializeField] private int extraSpawnEveryXLevels = 5;
    [SerializeField] private float spawnRadius = 1.5f;

    public void SpawnTinyEnemies()
    {
        if (tinyEnemyPrefab == null) return;

        int amount = baseSpawnAmount + bossLevel / extraSpawnEveryXLevels;

        for (int i = 0; i < amount; i++)
        {
            Vector2 randOffset = Random.insideUnitCircle.normalized * spawnRadius;
            if (PoolRouter.Instance != null)
                PoolRouter.Instance.GetFromPool(tinyEnemyPrefab, (Vector2)transform.position + randOffset, Quaternion.identity);
            else
                Instantiate(tinyEnemyPrefab, (Vector2)transform.position + randOffset, Quaternion.identity);
        }
    }
}
