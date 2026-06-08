using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MeleeOrbitWeapon : MonoBehaviour
{
    [SerializeField] private int       baseDamage     = 1;
    [SerializeField] private float     damageCooldown = 0.5f; //seconds before hitting same enemy again.
    [SerializeField] private LayerMask enemyLayer;

    public UnityEvent OnHit;

    // track last hit time per enemy so one pass doesn't spam damage every frame
    private readonly Dictionary<Collider2D, float> lastHitTime = new();

    private void OnTriggerStay2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        if (lastHitTime.TryGetValue(other, out float t) && Time.time - t < damageCooldown) return;

        lastHitTime[other] = Time.time;

        Health health = other.GetComponent<Health>();
        if (health == null) return;

        float multiplier = PlayerStats.Instance != null ? PlayerStats.Instance.DamageMultiplier : 1f;
        health.TakeDamage(Mathf.Max(1, Mathf.RoundToInt(baseDamage * multiplier)));
        OnHit?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // clean up enemies that leave the trigger so we don't keep tracking them forever
        lastHitTime.Remove(other);
    }
}
