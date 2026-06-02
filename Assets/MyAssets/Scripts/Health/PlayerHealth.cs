using UnityEngine;

public class PlayerHealth : Health
{
    [Header("Player Damage")]
    [SerializeField] private float damageCooldown = 0.5f;

    [Header("Camera Shake")]
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private float damageShakeIntensity = 1f;
    [SerializeField] private float damageShakeDuration = 0.15f;

    private float lastDamageTime;
    private PlayerController playerController;

    protected override void Awake()
    {
        base.Awake();
        playerController = GetComponent<PlayerController>();

        if(cameraShake == null)
            cameraShake = FindFirstObjectByType<CameraShake>();
    }

    public override void TakeDamage(int damage)
    {   
        if(isDead) return;

        if(Time.time < lastDamageTime + damageCooldown)
            return;
        
        if(playerController != null && playerController.IsInvulnerable)
            return;
        
        lastDamageTime = Time.time;

        cameraShake?.Shake(damageShakeIntensity, damageShakeDuration);

        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();
        print("[PlayerHealth] Player Died.");
    }
}
