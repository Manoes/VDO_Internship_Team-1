public class WormHeadHealth : Health
{
    private WormBoss wormBoss;

    protected override void Awake()
    {
        base.Awake();

        wormBoss = GetComponent<WormBoss>();
    }

    public override void TakeDamage(int damage)
    {
        if(wormBoss == null) return;

        wormBoss.DamageWormHead();
    }
}