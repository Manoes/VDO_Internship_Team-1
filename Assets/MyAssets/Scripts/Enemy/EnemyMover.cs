using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private bool rotateEnemy = true;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 0.2f;

    private Transform player;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if(playerObject != null)
            player = playerObject.transform;
    }

    void FixedUpdate()
    {
        if(player == null) return;

        Vector2 direction = player.position - transform.position;

        if(direction.magnitude <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        direction.Normalize();

        rb.linearVelocity = direction.normalized * moveSpeed;
        
        if(rotateEnemy)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }
}
