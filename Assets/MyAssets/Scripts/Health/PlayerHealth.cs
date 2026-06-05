using System.Collections;
using UnityEngine;

public class PlayerHealth : Health
{
    [Header("Player Damage")]
    [SerializeField] private float damageCooldown = 0.5f;

    [Header("Camera Shake")]
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private float damageShakeIntensity = 1f;
    [SerializeField] private float damageShakeDuration = 0.15f;

    [Header("Death Slow Motion")]
    [SerializeField] private float deathSlowMoDuration = 0.4f;
    [SerializeField] private float finalTimeScale = 0f;

    private float lastDamageTime;
    private PlayerController playerController;

    protected override void Awake()
    {
        base.Awake();
        playerController = GetComponent<PlayerController>();

        if (cameraShake == null)
            cameraShake = FindFirstObjectByType<CameraShake>();
    }

    public override void TakeDamage(int damage)
    {
        TakeDamage(damage, transform.position);
    }

    public override void TakeDamage(int damage, Vector2 hitPoint)
    {
        if (isDead) return;

        if (Time.time < lastDamageTime + damageCooldown)
            return;

        if (playerController != null && playerController.IsInvulnerable)
            return;

        lastDamageTime = Time.time;

        cameraShake?.Shake(damageShakeIntensity, damageShakeDuration);

        base.TakeDamage(damage, hitPoint);
    }

    protected override void Die()
    {
        base.Die();
        print("[PlayerHealth] Player Died.");

        StartCoroutine(DeathSlowMoRoutine());
    }

    private IEnumerator DeathSlowMoRoutine()
    {
        float startScale = Time.timeScale;
        float timer = 0f;

        while (timer < deathSlowMoDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / deathSlowMoDuration;
            Time.timeScale = Mathf.Lerp(startScale, finalTimeScale, t);

            yield return null;
        }

        Time.timeScale = finalTimeScale;
    }
}
