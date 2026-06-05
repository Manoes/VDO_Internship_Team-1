using UnityEngine;

public class WormBodyPart : Health
{
    private WormBoss wormBoss;

    public void Initialize(WormBoss boss)
    {
        wormBoss = boss;
    }

    protected override void Die()
    {
        wormBoss?.DamageWormBody();
    }
}