using UnityEngine;
using UnityEngine.InputSystem;

public class BossTestSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Bosses")]
    [SerializeField] private GameObject wormBossPrefab;
    [SerializeField] private GameObject trojanBossPrefab;
    [SerializeField] private GameObject ddosBossPrefab;
    [SerializeField] private GameObject hackerBossPrefab;

    [Header("Test Settings")]
    [SerializeField] private int testBossLevel = 5;
    [SerializeField] private float spawnDistance = 6f;

    private GameObject currentBoss;

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
        if (Keyboard.current == null) return;

        if (Keyboard.current.f1Key.wasPressedThisFrame)
            SpawnBoss(wormBossPrefab);

        if (Keyboard.current.f2Key.wasPressedThisFrame)
            SpawnBoss(trojanBossPrefab);

        if (Keyboard.current.f3Key.wasPressedThisFrame)
            SpawnBoss(ddosBossPrefab);

        if (Keyboard.current.f4Key.wasPressedThisFrame)
            SpawnBoss(hackerBossPrefab);

        if (Keyboard.current.xKey.wasPressedThisFrame)
            ClearBoss();
    }

    private void SpawnBoss(GameObject bossPrefab)
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning("[BossTestSpawner] Boss Prefab Missing.");
            return;
        }

        if(player == null)
        {
            Debug.LogWarning("[BossTestSpawner] No Player found. Make sure Player has tag 'Player'.");
            return;
        }

        ClearBoss();

        Vector2 spawnPosition = (Vector2)player.position + Vector2.right * spawnDistance;

        currentBoss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);

        if (currentBoss.TryGetComponent<BossBase>(out BossBase bossBase))
            bossBase.Initialize(testBossLevel);

        print($"[BossTestSpawner] Spawned {bossPrefab.name} at test level: {testBossLevel}");
    }

    private void ClearBoss()
    {
        if (currentBoss != null)
            Destroy(currentBoss);
    }
}
