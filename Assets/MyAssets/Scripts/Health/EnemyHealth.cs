using UnityEngine;

public class EnemyHealth : Health
{
    private EnemySpawner spawner;

    public void Initialize(EnemySpawner spawner)
    {
        this.spawner = spawner;
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
