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

    [Header("Glitch")]
    [SerializeField] private bool useGlitch = true;
    [SerializeField] private float glitchInterval = 0.06f;
    [SerializeField] private float jitterAmount = 0.08f;
    [SerializeField] private float scaleGlitchAmount = 0.12f;

    [SerializeField] private Color[] glitchColors =
    {
        new Color32(57, 255, 20, 255),
        new Color32(0, 255, 255, 255),
        new Color32(255, 0, 255, 255),
        new Color32(255, 255, 255, 255)
    };

    [Header("Audio")]
    [SerializeField] private AudioSource loopAudio;

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color defaultColor = new Color32(57, 255, 20, 255);

    private float tickTimer;
    private float glitchTimer;

    private Vector3 basePosition;
    private Vector3 baseScale;

    private Tween pulseTween;
    private Tween colorTween;
    private Tween lifeTimeTween;

    private bool despawning;

    void Awake()
    {
        if(spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        tickTimer = 0f;
        glitchTimer = 0f;
        despawning = false;

        basePosition = transform.position;
        baseScale = Vector3.one;

        KillTweens();

        transform.localScale = Vector3.zero;

        if(spriteRenderer != null)
            spriteRenderer.color = defaultColor;

        loopAudio?.Play();

        transform
            .DOScale(baseScale, spawnDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(StartPulseAnimation);

        lifeTimeTween = DOVirtual.DelayedCall(
            lifeTime,
            DespawnZone,
            true
        );
    }

    void Update()
    {
        if(!useGlitch) return;
        if(despawning) return;

        glitchTimer -= Time.deltaTime;

        if(glitchTimer > 0f) return;

        glitchTimer =  Random.Range(glitchInterval * 0.5f, glitchInterval * 1.5f);

        ApplyGlitch();
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
        loopAudio?.Stop();
        KillTweens();

        if(spriteRenderer != null)
            spriteRenderer.color = defaultColor;
        
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }

    private void ApplyGlitch()
    {
        Vector2 jitter = Random.insideUnitCircle * jitterAmount;
        transform.position = basePosition + new Vector3(jitter.x, jitter.y, 0f);

        float xScale = Random.Range(1f - scaleGlitchAmount, 1f + scaleGlitchAmount);
        float yScale = Random.Range(1f - scaleGlitchAmount, 1f + scaleGlitchAmount);

        transform.localScale = new Vector3(
            baseScale.x * xScale,
            baseScale.y * yScale,
            baseScale.z
        );

        if(spriteRenderer != null && glitchColors.Length > 0)
            spriteRenderer.color = glitchColors[Random.Range(0, glitchColors.Length)];
    }

    #region Animations

    private void StartPulseAnimation()
    {
        if(despawning) return;

        pulseTween = transform
            .DOScale(pulseScale, pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        
        if(spriteRenderer != null)
        {
            colorTween = spriteRenderer
                .DOColor(defaultColor, pulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }        

    private void DespawnZone()
    {
        if(despawning) return;

        despawning = true;

        KillPulseTweens();

        transform.position = basePosition;

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

    private void KillTweens()
    {
        pulseTween?.Kill();
        colorTween?.Kill();
        lifeTimeTween?.Kill();

        transform.DOKill();

        if(spriteRenderer != null)
            spriteRenderer.DOKill();
    }

    private void KillPulseTweens()
    {
        pulseTween?.Kill();
        colorTween?.Kill();

        if(spriteRenderer != null)
            spriteRenderer.DOKill();
    }

    #endregion
}
