using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 5;

    [SerializeField, InspectorReadOnly] protected int currentHealth;
    protected bool isDead;

    [Header("Hit VFX")]
    [SerializeField] private GameObject hitVFXPrefab;

    [Header("Damage VFX")]
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    [SerializeField] private Color damageColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Death VFX")]
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private Transform visualRoot;
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
        if (visualRoot == null)
            visualRoot = transform;

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
        visualRoot.DOKill();
        visualRoot.localScale = Vector3.one;

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

    public void SetMaxHealth(int newMaxHealth, bool refillHealth)
    {
        maxHealth = Mathf.Max(1, newMaxHealth);

        if (refillHealth)
            currentHealth = maxHealth;
        else
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public virtual void TakeDamage(int damage)
    {
        TakeDamage(damage, transform.position);
    }

    public virtual void TakeDamage(int damage, Vector2 position)
    {
        if (isDead) return;
        if (damage <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        DamageFlash();
        SpawnHitVFX(position);
        SpawnDamageNumber(damage, position);

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

    private void SpawnHitVFX(Vector2 hitpoint)
    {
        if (hitVFXPrefab == null) return;

        if (PoolRouter.Instance != null)
            PoolRouter.Instance.GetFromPool(hitVFXPrefab, hitpoint, Quaternion.identity);
        else
            Instantiate(hitVFXPrefab, hitpoint, Quaternion.identity);
    }

    private void SpawnDamageNumber(int damage, Vector2 position)
    {
        if (damageNumberPrefab == null) return;

        Vector2 spawnPos = position + Random.insideUnitCircle * 0.25f;

        GameObject obj = PoolRouter.Instance != null
            ? PoolRouter.Instance.GetFromPool(damageNumberPrefab, spawnPos, Quaternion.identity)
            : Instantiate(damageNumberPrefab, spawnPos, Quaternion.identity);

        if (obj.TryGetComponent<DamageNumber>(out DamageNumber number))
            number.Initialize(damage);
    }

    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;

        if (visualRoot == null)
            visualRoot = transform;

        visualRoot.DOKill();

        Debug.Log($"[Health] Death pop started on {visualRoot.name}");

        Sequence deathSequence = DOTween.Sequence();
        deathSequence.SetUpdate(true);

        deathSequence.Append(
            visualRoot
                .DOScale(Vector3.one * popScale, popDuration * 0.5f)
                .SetEase(Ease.OutBack)
        );

        deathSequence.Append(
            visualRoot
                .DOScale(Vector3.zero, popDuration * 0.5f)
                .SetEase(Ease.InBack)
        );

        deathSequence.OnComplete(() =>
        {
            Debug.Log("[Health] Death pop complete");
            OnDeath?.Invoke();
        });
    }

    public virtual void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
