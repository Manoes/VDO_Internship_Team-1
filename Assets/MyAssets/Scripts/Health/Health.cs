using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 5;

    [SerializeField, InspectorReadOnly] protected int currentHealth;
    protected bool isDead;

    [Header("Damage VFX")]
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    [SerializeField] private Color damageColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Death VFX")]
    [SerializeField] private float popScale = 1.3f;
    [SerializeField] private float popDuration = 0.15f;

    [Header("Events")]
    public UnityEvent OnDamaged;
    public UnityEvent OnDeath;
    public UnityEvent<int, int> OnHealthChanged; // Current, max

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private Color[] originalColors;

    void OnEnable()
    {
        ResetHealth();
    }

    protected virtual void Awake()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
            originalColors[i] = spriteRenderers[i].color;
    }

    protected virtual void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void ResetHealth()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;

        isDead = false;
        currentHealth = maxHealth;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null) continue;

            spriteRenderers[i].DOKill();
            spriteRenderers[i].color = originalColors[i];
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;
        if (damage <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        DamageFlash();
        OnDamaged?.Invoke();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    private void DamageFlash()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null) return;

            spriteRenderers[i].DOKill();

            spriteRenderers[i].color = damageColor;

            spriteRenderers[i]
                .DOColor(originalColors[i], flashDuration)
                .SetEase(Ease.OutQuad);
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;

        transform
            .DOScale(popScale, popDuration * 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                transform
                    .DOScale(0f, popDuration * 0.5f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() =>
                    {
                        OnDeath?.Invoke();
                    });
            });

        isDead = true;
        OnDeath?.Invoke();
    }

    public virtual void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
