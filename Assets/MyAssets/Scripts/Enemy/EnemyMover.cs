using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMover : MonoBehaviour, ILevelScalable
{
    [Header("Movement")]
    [SerializeField] private bool rotateEnemy = true;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float speedIncreasePerLevel = 0.05f;
    [SerializeField] private float maxMoveSpeed = 7f;
    [SerializeField] private float stopDistance = 0.2f;

    [Header("Seperation")]
    [SerializeField] private float seperationRadius = 1f;
    [SerializeField] private float seperationStrength = 2f;

    private float baseMoveSpeed;
    private Transform player;
    private Rigidbody2D rb;

    void Awake()
    {
        baseMoveSpeed = moveSpeed;
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
    }

    public void ApplyLevelScaling(int level)
    {
        moveSpeed = Mathf.Min(baseMoveSpeed + (level - 1) * speedIncreasePerLevel, maxMoveSpeed);
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 direction = player.position - transform.position;

        if (direction.magnitude <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 moveDirection =
            ((Vector2)(player.position - transform.position)).normalized;

        moveDirection += GetSeperationForce();

        rb.linearVelocity =
            moveDirection.normalized * moveSpeed;

        if (rotateEnemy)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }

    private Vector2 GetSeperationForce()
    {
        Collider2D[] neighbours = Physics2D.OverlapCircleAll(transform.position, seperationRadius);

        Vector2 force = Vector2.zero;

        foreach (Collider2D neighbour in neighbours)
        {
            if (neighbour.gameObject == gameObject)
                continue;

            if (!neighbour.CompareTag("Enemy"))
                continue;

            Vector2 away = (Vector2)(transform.position - neighbour.transform.position);

            float distance = away.magnitude;

            if (distance > 0.01f)
                force += away.normalized / distance;
        }

        return force * seperationStrength;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, seperationRadius);
    }
}
