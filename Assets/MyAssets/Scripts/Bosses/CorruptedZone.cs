using DG.Tweening;
using UnityEngine;

public class CorruptedZone : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damagePerSecond = 1;
    [SerializeField] private float tickRate = 0.5f;

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 4f;

    [Header("Animation")]
    [SerializeField] private float spawnDuration = 0.25f;
    [SerializeField] private float despawnDuration = 0.25f;

    [SerializeField] private float pulseScale = 1.1f;
    [SerializeField] private float pulseDuration = 0.5f;

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color pulseColor = Color.red;

    private float tickTimer;

    private Tween pulseTween;

    void OnEnable()
    {
        tickTimer = 0f;

        transform.localScale = Vector3.zero;

        transform
            .DOScale(1f, spawnDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(StartPulseAnimation);

        DOVirtual.DelayedCall(
            lifeTime,
            DespawnZone,
            true
        );
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        tickTimer -= Time.deltaTime;

        if (tickTimer > 0f) return;

        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damagePerSecond);
            tickTimer = tickRate;
            print($"[CorruptedZone] Dealing {damagePerSecond} damage to player");
        }
    }

    void OnDisable()
    {
        pulseTween?.Kill();
        transform.DOKill();
    }

    #region Animations

    private void StartPulseAnimation()
    {
        pulseTween?.Kill();

        spriteRenderer
            .DOColor(pulseColor, pulseDuration)
            .SetLoops(-1, LoopType.Yoyo);

        pulseTween = transform
            .DOScale(pulseScale, pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void DespawnZone()
    {
        pulseTween?.Kill();

        transform
            .DOScale(0f, despawnDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                if (PoolRouter.Instance != null)
                    PoolRouter.Instance.ReturnToPool(gameObject);
                else
                    Destroy(gameObject);
            });
    }

    #endregion
}
