using UnityEngine;

public class HackerBoss : BossBase
{
    [Header("Audio")]
    [SerializeField] private AudioSource spawnAudio;

    [Header("Zone")]
    [SerializeField] private GameObject curruptedZonePrefab;

    [Header("Spawn")]
    [SerializeField] private float spawnRangeAroundPlayer = 6f;
    [SerializeField] private float baseSpwanInterval = 2f;
    [SerializeField] private float minSpawnInterval = 0.75f;
    [SerializeField] private float intervalDecreasePerLevel = 0.05f;

    private Transform player;
    private float spawnTimer;

    public override void Initialize(int level)
    {
        base.Initialize(level);

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
    }

    void Update()
    {
        if (player == null) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnCorruptedZone();
            spawnTimer = GetSpawnInterval();
        }
    }

    private void SpawnCorruptedZone()
    {
        spawnAudio?.PlayOneShot(spawnAudio.clip);

        Vector2 spawnDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = player.position + (Vector3)(spawnDirection * spawnRangeAroundPlayer);

        if (PoolRouter.Instance != null)
            PoolRouter.Instance.GetFromPool(curruptedZonePrefab, spawnPosition, Quaternion.identity);
        else
            Instantiate(curruptedZonePrefab, spawnPosition, Quaternion.identity);
    }

    private float GetSpawnInterval()
    {
        float interval = baseSpwanInterval - bossLevel * intervalDecreasePerLevel;
        return Mathf.Max(interval, minSpawnInterval);
    }
}
