using UnityEngine;

public class EnemyHealth : Health
{
    protected override void Die()
    {
        base.Die();

        if (PoolRouter.Instance != null)
            PoolRouter.Instance.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }
}
