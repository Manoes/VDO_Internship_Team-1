using Unity.Mathematics;
using UnityEngine;

public class HelperOrbit : MonoBehaviour
{
    private Transform target;
    private float radius, speed, angle;

    public void Initialize(Transform orbitTarget, float initialAngle, float orbitRadius, float orbitSpeed)
    {
        target = orbitTarget;
        angle  = initialAngle;
        radius = orbitRadius;
        speed  = orbitSpeed;
    }

    public void SetAngleOffset(float newAngle) => angle = newAngle;
    
    private void Update()
    {
        if (target == null) return;
        angle += speed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;
        transform.position = target.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
    }
}
