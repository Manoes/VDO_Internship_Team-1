using UnityEngine;

public class GameLevelConnector : MonoBehaviour
{
   [SerializeField] private PlayerLevelSystem levelSystem;
   [SerializeField] private EnemySpawner enemySpawner;

    void Awake()
    {
        levelSystem.OnLevelUp.AddListener(enemySpawner.OnPlayerLevelUp);
    }

    void OnDestroy()
    {
        levelSystem.OnLevelUp.RemoveListener(enemySpawner.OnPlayerLevelUp);
    }
}
