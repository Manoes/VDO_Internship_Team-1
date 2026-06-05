using System.Collections.Generic;
using UnityEngine;

public class WormBoss : BossBase
{
    [Header("Body")]
    [SerializeField] private GameObject bodyPartPrefab;
    [SerializeField] private int baseBodyParts = 6;
    [SerializeField] private int extraBodyPartEveryXLevels = 5;
    [SerializeField] private float bodySpacing = 0.75f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 8f;
    [SerializeField] private float bodyFollowSpeed = 10f;

    [Header("Head Health")]
    [SerializeField] private int headHitsAfterBodyDestroyed = 3;
    [SerializeField] private int xpReward = 25;

    private bool dead;

    private readonly List<WormBodyPart> bodyParts = new();

    private Transform player;
    private Vector2 moveDirection = Vector2.right;
    private int currentHeadBits;
    private bool headVulnerable;

    public override void Initialize(int level)
    {
        base.Initialize(level);

        dead = false;
        DespawnAllBodyparts();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;

        SpawnBody();

        currentHeadBits = headHitsAfterBodyDestroyed;
        headVulnerable = false;
    }

    private void Update()
    {
        if (player == null) return;

        Vector2 targetDirection = (player.position - transform.position).normalized;
        moveDirection = Vector2.Lerp(moveDirection, targetDirection, turnSpeed * Time.deltaTime).normalized;

        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        UpdateBodyPositions();
    }

    private void SpawnBody()
    {
        int extraParts = bossLevel / extraBodyPartEveryXLevels;
        int totalParts = baseBodyParts + extraParts;

        for (int i = 0; i < totalParts; i++)
        {
            Vector2 position = (Vector2)transform.position - moveDirection * bodySpacing * i;

            GameObject bodyPart = PoolRouter.Instance != null
                ? PoolRouter.Instance.GetFromPool(bodyPartPrefab, position, Quaternion.identity)
                : Instantiate(bodyPartPrefab, position, Quaternion.identity);

            WormBodyPart part = bodyPart.GetComponent<WormBodyPart>();
            part.Initialize(this);

            bodyParts.Add(part);
        }
    }

    private void UpdateBodyPositions()
    {
        Vector2 previousPosition = transform.position;

        for (int i = 0; i < bodyParts.Count; i++)
        {
            if (bodyParts[i] == null) continue;

            Transform partTransform = bodyParts[i].transform;

            Vector2 toPrevious = previousPosition - (Vector2)partTransform.position;
            float distance = toPrevious.magnitude;

            if (distance > bodySpacing)
            {
                Vector2 targetPosition =
                    previousPosition - toPrevious.normalized * bodySpacing;

                partTransform.position = Vector2.Lerp(
                    partTransform.position,
                    targetPosition,
                    bodyFollowSpeed * Time.deltaTime
                );
            }

            Vector2 lookDirection = previousPosition - (Vector2)partTransform.position;

            if (lookDirection.magnitude > 0.001f)
            {
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                partTransform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
            }

            previousPosition = partTransform.position;
        }
    }

    public void DamageWormBody()
    {
        if (bodyParts.Count <= 0)
        {
            MakeHeadVulnerable();
            return;
        }

        WormBodyPart lastPart = bodyParts[^1];
        bodyParts.RemoveAt(bodyParts.Count - 1);

        if (lastPart != null)
        {
            if (PoolRouter.Instance != null)
                PoolRouter.Instance.ReturnToPool(lastPart.gameObject);
            else
                Destroy(lastPart.gameObject);
        }

        if (bodyParts.Count <= 0)
            MakeHeadVulnerable();
    }

    public void DamageWormHead()
    {
        if (!headVulnerable)
        {
            DamageWormBody();
            return;
        }

        currentHeadBits--;

        print($"[WormBoss] Head Hit. Hits Left: {currentHeadBits}");

        if (currentHeadBits <= 0)
            Die();
    }

    private void MakeHeadVulnerable()
    {
        if (headVulnerable) return;

        headVulnerable = true;

        print("[WormBoss] Head is now Vulnerable");
    }

    public void Die()
    {
        if (dead) return;
        dead = true;

        print("[WormBoss] Worm Defeated.");

        DespawnAllBodyparts();

        if (PlayerLevelSystem.Instance != null)
            PlayerLevelSystem.Instance.AddXP(xpReward);

        if (PoolRouter.Instance != null)
            PoolRouter.Instance.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }

    private void DespawnAllBodyparts()
    {
        for (int i = bodyParts.Count - 1; i >= 0; i--)
        {
            if (bodyParts[i] == null) continue;

            if (PoolRouter.Instance != null)
                PoolRouter.Instance.ReturnToPool(bodyParts[i].gameObject);
            else
                Destroy(bodyParts[i].gameObject);
        }

        bodyParts.Clear();
    }
}
