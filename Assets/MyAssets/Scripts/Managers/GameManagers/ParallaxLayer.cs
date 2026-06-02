using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Transform target; 
    [SerializeField] private float parallaxFactor = 0.2f;

    private Vector3 startPosition;
    private Vector3 targetStartPosition;

    private void Start()
    {
        if (target == null)
            target = Camera.main.transform; 

        startPosition = transform.position;
        targetStartPosition = target.position;
    }

    void LateUpdate()
    {
        Vector3 delta = target.position - targetStartPosition;

        transform.position = startPosition + delta * parallaxFactor;
    }
}
