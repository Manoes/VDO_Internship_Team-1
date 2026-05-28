using UnityEngine;

public class PooledProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected float lifetime = 5f;

    protected bool isActiveProjectile;
    private float despawnTime;

    protected virtual void OnEnable()
    {
        // Safety Reset when Reused from Pool
        isActiveProjectile = false;
    }

    protected virtual void Update()
    {
        if(!isActiveProjectile) return;

        if(Time.time >= despawnTime)
            Despawn();
    }

    protected void StartLifetime()
    {
        isActiveProjectile = true;
        despawnTime = Time.time + lifetime;
    }

    protected virtual void Despawn()
    {
        isActiveProjectile = false;

        if(PoolRouter.Instance != null)
            PoolRouter.Instance.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }
}