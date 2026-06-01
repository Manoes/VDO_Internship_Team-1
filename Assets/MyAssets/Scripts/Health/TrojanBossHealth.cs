public class TrojanBossHealth : Health
{
    private TrojanBoss trojanBoss;

    protected override void Awake()
    {
        base.Awake();

        trojanBoss = GetComponent<TrojanBoss>();
    }

    protected override void Die()
    {
        trojanBoss?.SpawnTinyEnemies();
        base.Die();

        if (PoolRouter.Instance != null)
            PoolRouter.Instance.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }
}