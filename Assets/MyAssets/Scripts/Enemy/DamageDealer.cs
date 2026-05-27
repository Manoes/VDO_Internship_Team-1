using UnityEngine;

public class DamageDealer : MonoBehaviour
{
   [SerializeField] private int damage = 1;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();

        if(health != null)
            health.TakeDamage(damage);
    }
}
