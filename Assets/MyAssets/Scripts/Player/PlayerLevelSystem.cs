using UnityEngine;
using UnityEngine.Events;

public class PlayerLevelSystem : Singleton<PlayerLevelSystem>
{
    [Header("Level")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentXP = 0;
    [SerializeField] private int xpToNextLevel = 10;

    [Header("XP Scaling")]
    [SerializeField] private float xpRrequirementMultiplier = 1.35f;
    [SerializeField] private int xpRrequirementFlatIncrease = 5;

    [Header("Events")]
    public UnityEvent<int> OnLevelChanged;      // Send new Level to UI and other systems that care about current level (e.g. for scaling damage, etc)
    public UnityEvent<int, int> OnXPChanged;    // CurrentXP, xpToNextLevel, for UI display
    public UnityEvent<int> OnLevelUp;           // Send new Level to Spawner, used by Spawner to determine which enemies to spawn and how many and also to trigger Upgrade Selection

    public int CurrentLevel => currentLevel;
    public int CurrentXP => currentXP;
    public int XPToNextLevel => xpToNextLevel;

    private bool waitingForUpgrade;

    void Start()
    {
        OnLevelChanged?.Invoke(currentLevel);
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
    }

    // Give Player XP. If they have enough XP to level up, level up and carry over excess XP. Repeat if multiple levels are gained at once.
    public void AddXP(int amount)
    {
        if(amount <= 0) return;

        currentXP += amount;

        if(!waitingForUpgrade && currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
    }

    private void LevelUp()
    {
        waitingForUpgrade = true;

        currentLevel++;

        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpRrequirementMultiplier + xpRrequirementFlatIncrease);

        print($"[PlayerLevelSystem] Leveled Up! Now to Level: {currentLevel}.");

        OnLevelChanged?.Invoke(currentLevel);
        OnLevelUp?.Invoke(currentLevel);
    }

    public void ContinueAfterUpgrade()
    {
        waitingForUpgrade = false;

        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
    }
}
