using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : PooledProjectile
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private int damage = 1;

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
        transform.rotation = Quaternion.Euler(0f, 0f, angle-90f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();

        if (health == null) return;

        health.TakeDamage(damage);

        Despawn();
    }

    protected override void Despawn()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        base.Despawn();
    }
}
