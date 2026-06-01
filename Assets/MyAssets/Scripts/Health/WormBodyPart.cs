public class WormBodyPart : Health
{
    private WormBoss wormBoss;

    public void Initialize(WormBoss boss)
    {
        wormBoss = boss;
    }

    public override void TakeDamage(int damage)
    {
        if(wormBoss != null)
            wormBoss.DamageWormBody();
    }
}