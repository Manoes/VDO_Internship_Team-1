using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float parallaxFactor = 0.2f;

    private Vector3 startPosition;
    private Vector3 targetStartPosition;

    private void Start()
    {
        if (target == null && Camera.main != null)
            target = Camera.main.transform;

        startPosition = transform.position;
        targetStartPosition = target.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 delta = target.position - targetStartPosition;

        transform.position = new Vector3(
            target.position.x + delta.x * parallaxFactor,
            target.position.y + delta.y * parallaxFactor,
            transform.position.z
        );
    }
}