using UnityEngine;

public abstract class BossBase : MonoBehaviour
{
    protected int bossLevel;

    public virtual void Initialize(int level)
    {
        bossLevel = level;
    }
}
