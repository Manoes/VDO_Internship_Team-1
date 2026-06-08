using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class BombOrbitWeapon : MonoBehaviour
{
    [SerializeField] private int       baseDamage       = 3;
    [SerializeField] private float     explosionRadius   = 1.5f;
    [SerializeField] private float     respawnDelay      = 3f;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private GameObject explosionVFXPrefab;

    public UnityEvent OnExplode;

    private SpriteRenderer spriteRenderer;
    private Collider2D     col;
    private bool           isArmed = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col            = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isArmed) return;
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        Explode();
    }

    private void Explode()
    {
        isArmed = false;

        float multiplier = PlayerStats.Instance != null ? PlayerStats.Instance.DamageMultiplier : 1f;
        int dmg          = Mathf.Max(1, Mathf.RoundToInt(baseDamage * multiplier));

        //AoE hit every enemy in the explosion radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            Health h = hit.GetComponent<Health>();
            if (h != null) h.TakeDamage(dmg);
        }

        if (explosionVFXPrefab != null)
            Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);

        OnExplode?.Invoke();
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        // hide visuals and disable collision, but keep HelperOrbit running so position stays correct
        if (spriteRenderer) spriteRenderer.enabled = false;
        if (col) col.enabled                       = false;

        yield return new WaitForSeconds(respawnDelay);

        if (spriteRenderer) spriteRenderer.enabled = true;
        if (col) col.enabled                       = true;
        isArmed                                    = true;
    }
}
