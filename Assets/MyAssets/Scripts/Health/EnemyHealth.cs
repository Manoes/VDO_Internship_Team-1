using UnityEngine;

public class EnemyHealth : Health, ILevelScalable
{
    [SerializeField] private int healthIncreasePerLevel = 1;

    private EnemySpawner spawner;

    public void Initialize(EnemySpawner spawner)
    {
        this.spawner = spawner;
        transform.localScale = Vector2.one;
    }

    public void ApplyLevelScaling(int level)
    {
        SetMaxHealth(MaxHealth + (level - 1) * healthIncreasePerLevel, true);
    }

    protected override void Die()
    {
        base.Die();

        EnemyXPAward xpReward = GetComponent<EnemyXPAward>();

        if(xpReward != null && PlayerLevelSystem.Instance != null)
            PlayerLevelSystem.Instance.AddXP(xpReward.XPReward);

        if(spawner != null)
            spawner.RemoveEnemy(gameObject);

        if (PoolRouter.Instance != null)
            PoolRouter.Instance.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }
}
