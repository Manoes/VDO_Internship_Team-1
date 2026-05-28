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
    public UnityEvent<int> OnLevelChanged;
    public UnityEvent<int, int> OnXPChanged; // CurrentXP, xpToNextLevel
    public UnityEvent<int> OnLevelUp;

    public int CurrentLevel => currentLevel;
    public int CurrentXP => currentXP;
    public int XPToNextLevel => xpToNextLevel;

    void Start()
    {
        OnLevelChanged?.Invoke(currentLevel);
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
    }

    public void AddXP(int amount)
    {
        if(amount <= 0) return;

        currentXP += amount;

        while(currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
    }

    private void LevelUp()
    {
        currentLevel++;

        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpRrequirementMultiplier + xpRrequirementFlatIncrease);

        print($"[PlayerLevelSystem] Leveled Up! Now to Level: {currentLevel}.");

        OnLevelChanged?.Invoke(currentLevel);
        OnLevelUp?.Invoke(currentLevel);
    }
}
