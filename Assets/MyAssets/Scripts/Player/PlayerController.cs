using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movemnet")]
    [SerializeField] private float moveSpeed = 7f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1.5f;

    [Header("Dash VFX")]
    [SerializeField] private CinemachineImpulseSource dashImpulse;
    [SerializeField] private float dashImpulseforce = 1.5f;

    [Header("Dash Invulnerability")]
    [SerializeField] private bool InvulnerableDuringDash = true;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private LayerMask dashThroughLayers;

    [Header("Upgrade Selection")]
    [SerializeField] private UpgradeUI upgradeUI;

    [Header("Death")]
    [SerializeField] private PlayerHealth playerHealth;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.right;

    private bool isDashing;
    private bool canDash = true;
    private bool isInvulnerable;

    public bool IsInvulnerable => isInvulnerable;
    public bool IsDashing => isDashing;

    void Awake()
    {
        LockCursor();

        rb = GetComponent<Rigidbody2D>();

        if (playerCollider == null)
            playerCollider = GetComponent<Collider2D>();

        if (upgradeUI == null)
            upgradeUI = FindFirstObjectByType<UpgradeUI>();
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        rb.linearVelocity = moveInput * moveSpeed;
    }

    #region Movement Handling

    private IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        if (InvulnerableDuringDash)
            isInvulnerable = true;

        IgnoreDashThroughCollisions(true);

        rb.linearVelocity = lastMoveDirection * dashSpeed;

        Vector2 impulseDirection = -lastMoveDirection.normalized;

        dashImpulse.GenerateImpulse(
            new Vector3(
                impulseDirection.x,
                impulseDirection.y,
                0f
            ) * dashImpulseforce
        );

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        if (InvulnerableDuringDash)
            isInvulnerable = false;

        IgnoreDashThroughCollisions(false);

        rb.linearVelocity = moveInput * moveSpeed;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    private void IgnoreDashThroughCollisions(bool ignore)
    {
        if (playerCollider == null) return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 100f, dashThroughLayers);

        foreach (Collider2D col in colliders)
        {
            if (col == null) continue;
            Physics2D.IgnoreCollision(playerCollider, col, ignore);
        }

    }

    #endregion

    #region Input Handling

    public void OnMove(InputValue value)
    {
        if(playerHealth.IsDead) return;
        moveInput = value.Get<Vector2>();

        if (moveInput.sqrMagnitude > 0.01f)
            lastMoveDirection = moveInput.normalized;
    }

    public void OnDash(InputValue value)
    {
        if(playerHealth.IsDead) return;
        if (!value.isPressed) return;
        if (!canDash || isDashing) return;

        StartCoroutine(DashRoutine());
    }

    public void OnPickUpgrade1(InputValue value)
    {
        if(playerHealth.IsDead) return;
        if (!value.isPressed) return;
        upgradeUI?.PickByIndex(0);
    }

    public void OnPickUpgrade2(InputValue value)
    {
        if(playerHealth.IsDead) return;
        if (!value.isPressed) return;
        upgradeUI?.PickByIndex(1);
    }

    public void OnPickUpgrade3(InputValue value)
    {
        if(playerHealth.IsDead) return;
        if (!value.isPressed) return;
        upgradeUI?.PickByIndex(2);
    }

    #endregion
}
