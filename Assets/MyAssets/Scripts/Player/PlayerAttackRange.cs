using DG.Tweening;
using UnityEngine;

public class PlayerAttackRange : MonoBehaviour
{
    [Header("Range")]
    [SerializeField] private float attackRadius = 5f;
    [SerializeField] private Transform rangeVisual;

    [Header("Tweens")]
    [SerializeField] private float rangeTweenDuration = 0.25f;
    [SerializeField] private Ease rangeTweenEase = Ease.OutBack;

    public float AttackRadius => attackRadius;

    private Tween rangeTween;

    void Start()
    {
        SetRange(attackRadius);
    }

    public void IncreaseRange(float amount)
    {
        attackRadius += amount;
    }

    public void SetRange(float newRadius)
    {
        attackRadius = newRadius;
        TweenRangeVisual();
    }

    private void TweenRangeVisual()
    {
        if (rangeVisual == null) return;

        rangeTween?.Kill();

        Vector3 targetScale = Vector3.one * attackRadius * 2f;

        rangeTween = rangeVisual
            .DOScale(targetScale, rangeTweenDuration)
            .SetEase(rangeTweenEase);
    }
}
