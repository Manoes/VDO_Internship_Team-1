using UnityEngine;

public class DamageDealer : MonoBehaviour
{
   [SerializeField] private int damage = 1;

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerHealth health = collision.GetComponent<PlayerHealth>();

        if(health != null)
            health.TakeDamage(damage);
    }
}
