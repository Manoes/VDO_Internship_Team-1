using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float moveUpAmount = 0.8f;
    [SerializeField] private float duration = 0.5f;

    private Sequence sequence;

    void Awake()
    {
        if (text == null)
            text = GetComponent<TextMeshProUGUI>();
    }

    public void Initialize(int damage)
    {
        text.text = damage.ToString();

        sequence?.Kill();

        transform.localScale = Vector3.one;
        text.alpha = 1f;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * moveUpAmount;

        sequence = DOTween.Sequence();
        sequence.Append(
            transform.DOScale(1.3f, 0.1f)
            .SetEase(Ease.OutBack)
        );

        sequence.Append(
           transform.DOScale(1f, 0.1f)
       );

        sequence.Join(
            transform.DOMove(endPos, duration)
            .SetEase(Ease.OutQuad)
        );

        sequence.Join(text.DOFade(0f, duration));

        sequence.OnComplete(Despawn);
    }

    private void Despawn()
    {
        if (PoolRouter.Instance != null)
            PoolRouter.Instance.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }
}
