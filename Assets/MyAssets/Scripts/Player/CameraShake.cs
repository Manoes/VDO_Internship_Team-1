using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;

    private CinemachineBasicMultiChannelPerlin noise;
    private Coroutine shakeRoutine;

    void Awake()
    {
        if(cam == null)
            cam = GetComponent<CinemachineCamera>();

        noise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake(float intensity, float duration)
    {
        if(noise == null) return;

        if(shakeRoutine != null)
            StopCoroutine(shakeRoutine);
        
        shakeRoutine = StartCoroutine(ShakeRoutine(intensity, duration));
    }

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        noise.AmplitudeGain = intensity;
        noise.FrequencyGain = 10f;

        yield return new WaitForSeconds(duration);

        noise.AmplitudeGain = 0f;
        shakeRoutine = null;
    }
}
