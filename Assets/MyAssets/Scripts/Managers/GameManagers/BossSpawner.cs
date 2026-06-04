using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Boss Pool")]
    [SerializeField] private GameObject[] bossPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 12f;

    private void Awake()
    {
        if(player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if(playerObject != null)
                player = playerObject.transform;
        }
    }

    public void SpawnRandomBoss(int level)
    {
        if(bossPrefabs == null || bossPrefabs.Length == 0) return;
        if(player == null) return;

        GameObject prefab = bossPrefabs[Random.Range(0, bossPrefabs.Length)];

        Vector2 direction = Random.insideUnitCircle.normalized;
        Vector2 spawnPosition = (Vector2)player.position + direction * spawnRadius;

        GameObject boss = PoolRouter.Instance != null
           ? PoolRouter.Instance.GetFromPool(prefab, spawnPosition, Quaternion.identity)
           : Instantiate(prefab, spawnPosition, Quaternion.identity);
        
        if(boss.TryGetComponent<BossBase>(out BossBase bossBase))
            bossBase.Initialize(level);
    }
}
