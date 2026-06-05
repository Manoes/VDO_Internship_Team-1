using DG.Tweening;
using UnityEngine;

public class EnemyDissolveDeathVFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    [Header("Dissolve VFX")]
    [SerializeField] private string dissolveProperty = "_DissolveSlider";
    [SerializeField] private float startValue = 0f;
    [SerializeField] private float endValue = 1f;
    [SerializeField] private float dissolveDuration = 0.35f;
    [SerializeField] private Ease dissolveEase = Ease.InQuad;

    private Material[] materialInstances;
    private Tween dissolveTween;

    void Awake()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        materialInstances = new Material[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null) continue;

            materialInstances[i] = new Material(spriteRenderers[i].sharedMaterial);
            spriteRenderers[i].material = materialInstances[i];
        }
    }

    void OnEnable()
    {
        ResetDissolve();
    }

    public void ResetDissolve()
    {
        dissolveTween?.Kill();

        if (materialInstances == null) return;

        foreach (Material mat in materialInstances)
        {
            if (mat == null) continue;
            mat.SetFloat(dissolveProperty, startValue);
        }
    }

    public void Play(System.Action onComplete)
    {
        dissolveTween?.Kill();

        float value = startValue;

        dissolveTween = DOTween.To(
                () => value,
                x =>
                {
                    value = x;

                    foreach (Material mat in materialInstances)
                    {
                        if (mat == null) continue;
                        mat.SetFloat(dissolveProperty, value);
                    }
                },
                endValue,
                dissolveDuration
            )
            .SetEase(dissolveEase)
            .OnComplete(() => onComplete?.Invoke());
    }

    private void OnDestroy()
    {
        dissolveTween?.Kill();

        if (materialInstances == null) return;

        foreach (Material mat in materialInstances)
        {
            if (mat != null)
                Destroy(mat);
        }
    }
}
