using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : PooledProjectile
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private int damage = 1;

    [Header("Damage Filter")]
    [SerializeField] private LayerMask damageLayers;

    private Rigidbody2D rb;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.Sleep();
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction)
    {
        StartLifetime();

        rb.linearVelocity = direction.normalized * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((damageLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        Health health = other.GetComponentInParent<Health>();

        if (health == null)
            return;

        int finalDamage = damage;

        if (health is EnemyHealth)
        {
            float multiplier = PlayerStats.Instance != null
                ? PlayerStats.Instance.DamageMultiplier
                : 1f;

            finalDamage = Mathf.Max(1, Mathf.RoundToInt(damage * multiplier));
        }

        Vector2 hitPoint = other.ClosestPoint(transform.position);

        health.TakeDamage(finalDamage, hitPoint);

        Despawn();
    }

    protected override void Despawn()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        base.Despawn();
    }
}
