using UnityEngine;

public class DamageDealer : MonoBehaviour, ILevelScalable
{
    [SerializeField] private int damage = 1;
    [SerializeField] private int damageIncreaseEveryXLevels = 5;
    [SerializeField] private int damageIncreaseAmount = 1;

    private int baseDamage;

    void Awake()
    {
        baseDamage = damage;
    }   

    public void ApplyLevelScaling(int level)
    {
        int bonus = (level - 1) / damageIncreaseEveryXLevels;
        damage = baseDamage + bonus * damageIncreaseAmount;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerHealth health = collision.GetComponent<PlayerHealth>();

        if (health != null)
            health.TakeDamage(damage);
    }
}
