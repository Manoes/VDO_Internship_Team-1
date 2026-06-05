using UnityEngine;

public class WormHeadHealth : Health
{
    private WormBoss wormBoss;

    protected override void Awake()
    {
        base.Awake();
        wormBoss = GetComponent<WormBoss>();
    }

    protected override void Die()
    {
        wormBoss?.DamageWormHead();
    }
}