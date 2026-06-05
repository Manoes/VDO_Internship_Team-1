using System.Collections.Generic;
using UnityEngine;

public class LaserOrbitWeapon : MonoBehaviour
{
    [SerializeField] private int       baseDamage       = 1;
    [SerializeField] private float     rotationSpeed    = 180f; // degrees per second
    [SerializeField] private float     damageCooldown   = 0.15f; //seconds before hitting same enemy again.
    [SerializeField] private LayerMask enemyLayer;

    private Transform playerTransform;
    private readonly Dictionary<Collider2D, float> lastHitTime = new();

    public void Initialize(Transform player)
    {
        playerTransform = player;
    }

    private void Update()
    {
        // follow the players position each frame (avoids parenting issues and keeps orbit centered on player)
        if (playerTransform != null)
            transform.position = playerTransform.position;

        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        if (lastHitTime.TryGetValue(other, out float t) && Time.time - t < damageCooldown) return;

        lastHitTime[other] = Time.time;

        EnemyHealth health = other.GetComponentInParent<EnemyHealth>();
        if (health == null) return;

        float multiplier = PlayerStats.Instance != null ? PlayerStats.Instance.DamageMultiplier : 1f;
        health.TakeDamage(Mathf.Max(1, Mathf.RoundToInt(baseDamage * multiplier)));
    }

    private void OnTriggerExit2D(Collider2D other) => lastHitTime.Remove(other);
}
