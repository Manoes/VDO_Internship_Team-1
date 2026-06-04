using DG.Tweening;
using UnityEngine;

public class EnemySpawnIndicator : MonoBehaviour
{
    [SerializeField] private float duration = 0.75f;

    [Header("Animation")]
    [SerializeField] private float overshootScale = 1.25f;
    [SerializeField] private float settleScale = 1f;
    [SerializeField] private float collapseScale = 0f;

    public float Duration => duration;

    private Sequence spawnSequence;

    void OnEnable()
    {
        spawnSequence?.Kill();

        transform.localScale = Vector3.zero;

        spawnSequence = DOTween.Sequence();

        spawnSequence.Append(
            transform.DOScale(overshootScale, duration * 0.4f)
            .SetEase(Ease.OutBack)
        );

        spawnSequence.Append(
            transform.DOScale(settleScale, duration * 0.25f)
            .SetEase(Ease.OutQuad)
        );

        spawnSequence.Append(
            transform.DOScale(collapseScale, duration * 0.35f)
            .SetEase(Ease.InBack)
        );
    }

    void OnDestroy()
    {
        spawnSequence?.Kill();
    }
}
