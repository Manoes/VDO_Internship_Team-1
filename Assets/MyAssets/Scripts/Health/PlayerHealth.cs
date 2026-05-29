using UnityEngine;

public class PlayerHealth : Health
{
    [Header("Player Damage")]
    [SerializeField] private float damageCooldown = 0.5f;

    private float lastDamageTime;
    private PlayerController playerController;

    protected override void Awake()
    {
        base.Awake();
        playerController = GetComponent<PlayerController>();
    }

    public override void TakeDamage(int damage)
    {   
        if(isDead) return;

        if(Time.time < lastDamageTime + damageCooldown)
            return;
        
        if(playerController != null && playerController.IsInvulnerable)
            return;
        
        lastDamageTime = Time.time;

        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();
        print("[PlayerHealth] Player Died.");
    }
}
