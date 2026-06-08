using UnityEngine;

public class EnemyHealth : Health, ILevelScalable
{
    [SerializeField] private int healthIncreasePerLevel = 1;

    private EnemySpawner spawner;
    private EnemyDissolveDeathVFX dissolveVFX;

    protected override void Awake()
    {
        base.Awake();
        dissolveVFX = GetComponent<EnemyDissolveDeathVFX>();
    }

    public void Initialize(EnemySpawner spawner)
    {
        this.spawner = spawner;
    }

    public void ApplyLevelScaling(int level)
    {
        SetMaxHealth(MaxHealth + (level - 1) * healthIncreasePerLevel, true);
    }

    protected override void Die()
    {
        if (IsDead) return;

        isDead = true;

        GiveXP();
        GiveScore();
        RemoveFromSpawner();
        OnDeath?.Invoke();

        if (dissolveVFX != null)
            dissolveVFX.Play(Despawn);
        else
            Despawn();

        ForceDeadState();
    }

    private void GiveXP()
    {
        EnemyXPAward xpReward = GetComponent<EnemyXPAward>();

        if (xpReward != null && PlayerLevelSystem.Instance != null)
            PlayerLevelSystem.Instance.AddXP(xpReward.XPReward);
    }

    private void GiveScore()
    {
        ScoreAward scoreAward = GetComponent<ScoreAward>();

        if (scoreAward != null && Timer.Instance != null)
            Timer.Instance.AddScore(scoreAward.ScoreAmount);
    }

    private void RemoveFromSpawner()
    {
        if (spawner != null)
            spawner.RemoveEnemy(gameObject);
    }

    private void Despawn()
    {
        if (PoolRouter.Instance != null)
            PoolRouter.Instance.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }

    private void ForceDeadState()
    {
        // ugly but fast: calls base damage protection by setting current health dead through base Die flow is annoying.
        // Better version would expose protected SetDead(), but deadline-wise:
        typeof(Health)
            .GetField("isDead", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(this, true);
    }
}
