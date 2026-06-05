using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Helpers")]
    [SerializeField] private GameObject helperPrefab;
    [SerializeField] private float helperOrbitRadius = 2f;
    [SerializeField] private float helperOrbitSpeed = 60f;

    // All multipliers start at 1.0 (100%)
    public float MoveSpeedMultiplier { get; private set; } = 1f;
    public float AttackSpeedMultiplier { get; private set; } = 1f;
    public float DamageMultiplier { get; private set; } = 1f;
    public float RangeMultiplier { get; private set; } = 1f;
    public float RegenPerSecond { get; private set; } = 0f;

    private int bonusMaxHealth;
    private float regenAccumulator;

    // Cached component refs
    private PlayerController playerController;
    private PlayerRandomAutoShooter playerShooter;
    private PlayerAttackRange playerAttackRange;
    private Health playerHealth;

    // Cached base values - read once from inspector values via reflection
    private float baseMoveSpeed;
    private float baseFireRate;
    private float baseAttackRadius;
    private int baseMaxHealth;

    private readonly List<GameObject> activeHelpers = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;

        playerController = GetComponent<PlayerController>();
        playerShooter = GetComponent<PlayerRandomAutoShooter>();
        playerAttackRange = GetComponent<PlayerAttackRange>();
        playerHealth = GetComponent<Health>();

        // Read the starting values set in the inspector
        baseMoveSpeed = ReflectionHelper.GetField<float>(playerController, "moveSpeed");
        baseAttackRadius = ReflectionHelper.GetField<float>(playerAttackRange, "attackRadius");
        baseFireRate = ReflectionHelper.GetField<float>(playerShooter, "fireRate");
        baseMaxHealth = ReflectionHelper.GetField<int>(playerHealth, "maxHealth");
    }

    private void OnDestroy() { if (Instance == this) Instance = null; }

    private void Update() => HandleRegen();

    // --------------- Called by upgrade SO's ---------------

    public void AddMoveSpeed(float bonus)
    {
        MoveSpeedMultiplier += bonus;
        // moveSpeed is read every FixedUpdate, so this takes effect next frame.
        ReflectionHelper.SetField(playerController, "moveSpeed", baseMoveSpeed * MoveSpeedMultiplier);
    }

    public void AddAttackSpeed(float bonus)
    {
        AttackSpeedMultiplier += bonus;
        // fireRate is read each time the timer resets, so takes effect on next shot
        ReflectionHelper.SetField(playerShooter, "fireRate", baseFireRate * AttackSpeedMultiplier);
        // HelperShooters read AttackSpeedMultiplier live, no patching needed.
    }

    public void AddDamage(float bonus)
    {
        DamageMultiplier += bonus;
        // ProjectileDamageApplier reads DamageMultiplier in OnEnable, takes effect on next spawn

    }

    public void AddMaxHealth(int amount)
    {
        bonusMaxHealth += amount;
        int newMax = baseMaxHealth + bonusMaxHealth;
        ReflectionHelper.SetField(playerHealth, "maxHealth", newMax);
        playerHealth.Heal(amount);
    }

    public void AddRange(float bonus)
    {
        RangeMultiplier += bonus;

        playerAttackRange.SetRange(baseAttackRadius * RangeMultiplier);
    }

    public void AddRegen(float regenPerSec) => RegenPerSecond += regenPerSec;

    public void AddHelper()
    {
        if (helperPrefab == null) { Debug.LogWarning("[PlayerStats] helperPrefab not assigned"); return; }
        GameObject helper = Instantiate(helperPrefab, transform.position, Quaternion.identity);
        activeHelpers.Add(helper);

        HelperOrbit orbit = helper.GetComponent<HelperOrbit>();
        if (orbit != null) orbit.Initialize(transform, 0f, helperOrbitRadius, helperOrbitSpeed);

        RespaceHelpers(); // evenly distribute orbit angles whenever count changes
    }

    private void RespaceHelpers()
    {
        int count = activeHelpers.Count;
        for (int i = 0; i < count; i++)
        {
            if (activeHelpers[i] == null) continue;
            HelperOrbit orbit = activeHelpers[i].GetComponent<HelperOrbit>();
            if (orbit != null) orbit.SetAngleOffset(i * (360f / count));
        }
    }

    // --------------- Regen ---------------

    private void HandleRegen()
    {
        if (RegenPerSecond <= 0f || playerHealth == null || playerHealth.IsDead) return;

        regenAccumulator += RegenPerSecond * Time.deltaTime;
        while (regenAccumulator >= 1f)
        {
            regenAccumulator -= 1f;
            playerHealth.Heal(1);
        }
    }
}
