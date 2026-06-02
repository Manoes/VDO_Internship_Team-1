using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;

        [Header("Unlock")]
        public int unlockLevel = 1;

        [Header("Weight")]
        public int baseWeight = 10;
        public int weightIncreasePerLevel = 1;
    }

    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Enemy Pool")]
    [SerializeField] private EnemySpawnData[] enemies;

    [Header("Spawning")]
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float baseSpawnInterval = 1.5f;
    [SerializeField] private float minSpawnInterval = 0.25f;
    [SerializeField] private int baseEnemiesPerSpawn = 1;
    [SerializeField] private int enemiesPerSpawnIncreaseEveryXLevels = 3;

    [Header("Spawn VFX")]
    [SerializeField] private GameObject spawnIndicatorPrefab;

    [Header("Events")]
    public UnityEvent OnUpgradeRequested;   // Open Upgrade UI and request a new upgrade selection and Open UI
    public UnityEvent OnCombatPaused;       // Lower Music Volume, show "Level Up!" text, etc
    public UnityEvent OnCombatResumed;      // When Upgrade is Selected and Combat can resume

    [Header("Difficulty")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float spawnIntervalDecreasePerLevel = 0.08f;

    private readonly List<GameObject> aliveEnemies = new();
    private float spawnTimer;
    private bool spawningEnabled = true;

    void Awake()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }
    }

    void Update()
    {
        if (!spawningEnabled) return;
        if (player == null) return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnWave();
            spawnTimer = GetSpawnInterval();
        }
    }

    private void SpawnWave()
    {
        int amount = GetEnemiesPerSpawn();

        for (int i = 0; i < amount; i++)
            StartCoroutine(SpawnEnemyRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        if(spawningEnabled == false) yield break;

        GameObject prefab = GetRandomEnemyForLevel();

        if (prefab == null) yield break;

        Vector2 spawnPosition = GetSpawnPosition();

        GameObject indicator = null;

        if(spawnIndicatorPrefab != null)
        {
            indicator = PoolRouter.Instance != null
                ? PoolRouter.Instance.GetFromPool(spawnIndicatorPrefab, spawnPosition, Quaternion.identity)
                : Instantiate(spawnIndicatorPrefab, spawnPosition, Quaternion.identity);
        }

        yield return new WaitForSeconds(indicator != null ? indicator.GetComponent<EnemySpawnIndicator>().Duration : 0f);

        if(indicator != null)
        {
            if (PoolRouter.Instance != null)
                PoolRouter.Instance.ReturnToPool(indicator);
            else
                Destroy(indicator);
        }

        if(spawningEnabled == false) yield break;

        GameObject enemy = PoolRouter.Instance != null
           ? PoolRouter.Instance.GetFromPool(prefab, spawnPosition, Quaternion.identity)
           : Instantiate(prefab, spawnPosition, Quaternion.identity);

        aliveEnemies.Add(enemy);

        if(enemy.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth))
            enemyHealth.Initialize(this);
    }

    private Vector2 GetSpawnPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        return (Vector2)player.position + randomDirection * spawnRadius;
    }

    private float GetSpawnInterval()
    {
        float interval = baseSpawnInterval - ((currentLevel - 1) * spawnIntervalDecreasePerLevel);
        return Mathf.Max(interval, minSpawnInterval);
    }

    private int GetEnemiesPerSpawn()
    {
        int bonus = (currentLevel - 1) / enemiesPerSpawnIncreaseEveryXLevels;
        return baseEnemiesPerSpawn + bonus;
    }
    
    // Decide which enemy to spawn based on current level and weights. 
    // Enemies that are unlocked longer have higher chances to spawn.
    private GameObject GetRandomEnemyForLevel()
    {
        int totalWeight = 0;

        foreach (EnemySpawnData enemy in enemies)
        {
            if (enemy.enemyPrefab == null) continue;
            if (currentLevel < enemy.unlockLevel) continue;

            totalWeight += GetEnemyWeight(enemy);
        }

        if (totalWeight <= 0) return null;

        int randomValue = Random.Range(0, totalWeight);

        foreach (EnemySpawnData enemy in enemies)
        {
            if (enemy.enemyPrefab == null) continue;
            if (currentLevel < enemy.unlockLevel) continue;

            int weight = GetEnemyWeight(enemy);

            if (randomValue < weight)
                return enemy.enemyPrefab;

            randomValue -= weight;
        }

        return null;
    }

    private int GetEnemyWeight(EnemySpawnData enemy)
    {
        int levelsAfterUnlock = Mathf.Max(0, currentLevel - enemy.unlockLevel);
        return enemy.baseWeight + (levelsAfterUnlock * enemy.weightIncreasePerLevel);
    }

    public void SetLevel(int level)
    {
        currentLevel = Mathf.Max(1, level);
    }

     // Called by PlayerLevelSystem when player levels up.
    public void OnPlayerLevelUp(int newLevel)
    {
        SetLevel(newLevel);

        StopSpawning();
        DespawnAllEnemies();

        OnCombatPaused?.Invoke();
        OnUpgradeRequested?.Invoke();
    }

    public void StopSpawning()
    {
        spawningEnabled = false;
    }

    public void ResumeSpawning()
    {
        spawningEnabled = true;
        spawnTimer = GetSpawnInterval();

        OnCombatResumed?.Invoke();
    }

    public void DespawnAllEnemies()
    {
        for (int i = aliveEnemies.Count - 1; i >= 0; i--)
        {
            if (aliveEnemies[i] != null && PoolRouter.Instance != null)
                PoolRouter.Instance.ReturnToPool(aliveEnemies[i]);
            else if(aliveEnemies[i] != null)
                Destroy(aliveEnemies[i]);
        }

        aliveEnemies.Clear();
    }

    public void RemoveEnemy(GameObject enemy)
    {
        aliveEnemies.Remove(enemy);
    }
}
