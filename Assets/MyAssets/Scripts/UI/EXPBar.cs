using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EXPBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private float fillDuration = 0.2f;

    private Tween fillTween;

    private void Start()
    {
        if (PlayerLevelSystem.Instance != null)
            UpdateXPBar(PlayerLevelSystem.Instance.CurrentXP, PlayerLevelSystem.Instance.XPToNextLevel);
    }

    public void UpdateXPBar(int currentXP, int maxXP)
    {
        if(fillImage == null || maxXP <= 0)
            return;

        float targetFill = (float)currentXP / maxXP;

        fillTween?.Kill();

        fillTween = fillImage
            .DOFillAmount(targetFill, fillDuration)
            .SetEase(Ease.OutQuad);
    }
}
