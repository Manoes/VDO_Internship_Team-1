using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 5;

    [SerializeField, InspectorReadOnly] protected int currentHealth;
    protected bool isDead;

    [Header("Events")]
    public UnityEvent OnDamaged;
    public UnityEvent OnDeath;
    public UnityEvent<int, int> OnHealthChanged; // Current, max

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    protected virtual void  Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public virtual void TakeDamage(int damage)
    {
        if(isDead) return;
        if(damage <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnDamaged?.Invoke();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if(currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        if(isDead) return;

        isDead = true;
        OnDeath?.Invoke();
    }

    public virtual void Heal(int amount)
    {
        if(isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
